# Cadence

Personal keyboard/piano practice tracker. Two microservices, **Repertoire**
(what I'm learning) and **Practice** (what I actually did), plus a shared
Contracts library. The two services never call each other over HTTP. They
talk only through one integration event carried over RabbitMQ.

---

## Running it

Requirements: .NET 10 SDK, Docker Desktop.

```bash
docker compose up -d
```

Brings up RabbitMQ (AMQP on `5672`, management UI on `15672`, login
`admin` / `admin`).

```bash
dotnet run --project src/Repertoire/Cadence.Repertoire.Api
dotnet run --project src/Practice/Cadence.Practice.Api
```

Run each in its own terminal. Nothing else to install. Each API applies its
own EF Core migration on startup (`Database.Migrate()` in `Program.cs`) and
creates its own SQLite file (`repertoire.db`, `practice.db`) next to the
project on first run.

| Service | Port | Health check |
|---|---|---|
| Repertoire.Api | `http://localhost:5001` | `GET /health` |
| Practice.Api | `http://localhost:5002` | `GET /health` |

Both `/health` endpoints check their own database and the RabbitMQ
connection. So a `200` from either one is a real signal, not just a "the
process is up" shrug. It means the whole chain (DB and bus) is actually
reachable.

To exercise every endpoint end to end, open [Cadence.http](Cadence.http) in
an editor with a REST client extension (VS Code's REST Client, Rider's HTTP
client, and so on) and run the requests top to bottom. It's built as one
linear walkthrough: create a piece, register a session against it, watch the
piece update, then hit the failure cases (409 on premature mastery, 404s, 400
on bad input).

For the UI, in a third terminal:

```bash
cd frontend
npm install
npm run dev
```

Serves on `http://localhost:5173`, the origin both APIs' CORS policy already
allows (RNF10). `frontend/src/config.ts` defaults to `5001`/`5002`; copy
`frontend/.env.example` to `frontend/.env.local` only if you need different
ports.

---

## Endpoints

**Repertoire.Api** (`:5001`)

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/pieces` | Create a piece. `201` + `Location`. |
| `GET` | `/pieces?status=&genre=` | List pieces, optionally filtered. |
| `GET` | `/pieces/{id}` | Fetch one piece with its practice record. `404` if missing. |
| `PUT` | `/pieces/{id}` | Update cataloguing details (title/composer/genre/difficulty/key/url). Cannot touch status or the practice record. |
| `PATCH` | `/pieces/{id}/status` | Manual status transition. `409` with a domain reason if illegal. |
| `DELETE` | `/pieces/{id}` | Remove a piece. `204`. |
| `GET` | `/health` | DB + RabbitMQ check. |

**Practice.Api** (`:5002`)

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/sessions` | Register a practice session. `201` + `Location`. Publishes `PracticeSessionRegistered`. Does **not** call Repertoire. |
| `GET` | `/sessions?pieceId=&from=&to=` | List sessions, newest first. |
| `GET` | `/sessions/{id}` | Fetch one session. `404` if missing. |
| `GET` | `/health` | DB + RabbitMQ check. |

Neither API paginates. Errors on both use RFC 9457 `application/problem+json`
via a shared `ResultExtensions.ToProblem()` shape (400 for invalid input, 409
for a rejected domain transition).

**One gotcha worth knowing before you hand-write a request body.** No
`JsonStringEnumConverter` is registered on either API. So `genre`,
`difficulty`, and `focus` in request bodies are plain integers matching the
enum's declared order (e.g. `Genre.Classical = 2`); see
[GenreEnum.cs](src/Repertoire/Cadence.Repertoire.Domain/Enums/GenreEnum.cs)
and
[DifficultyEnum.cs](src/Repertoire/Cadence.Repertoire.Domain/Enums/DifficultyEnum.cs).
`PATCH /pieces/{id}/status` is the one exception: its `status` field is a
string, parsed case-insensitively (`"Polishing"`, `"Mastered"`, and so on).
But response bodies always come back as strings (`piece.Status.ToString()`).
So what you read is never what you write for those fields. That's a real
asymmetry, not a typo.

---

## Frontend

React + TypeScript, talking to both APIs directly, no BFF. Two base URLs,
not one — `config.repertoireUrl` and `config.practiceUrl` in
[config.ts](frontend/src/config.ts), never a literal port anywhere else in
the client code.

The enum asymmetry described above (`Endpoints`) is encoded once, in
[enums.ts](frontend/src/domain/enums.ts): `genreToWireValue`,
`difficultyToWireValue`, and `practiceFocusToWireValue` turn the string
unions the rest of the UI works with into the integers the request bodies
need, applied right before the fetch call in
[repertoire.ts](frontend/src/api/repertoire.ts) and
[practice.ts](frontend/src/api/practice.ts). Response bodies need no
mapping — they're already strings. `ChangeStatusRequest.status` is the one
field sent as-is, a string, matching the backend's `Enum.TryParse`.

The other thing the UI can't paper over: after `POST /sessions` succeeds,
the piece's practice record is updated by the RabbitMQ consumer, not by
that request. No optimistic write pretends the record already caught up —
the piece detail view is expected to refetch on a short delay instead,
making the eventual consistency visible rather than hidden.
[`useRecordRefresh`](frontend/src/features/pieces/useRecordRefresh.ts)
invalidates the piece detail query and re-polls it a few times (800ms,
1.6s, 3s) after a session is logged, stopping early once `sessionCount`
goes up. It refetches rather than patches the record locally because the
incremental average in `PracticeRecord.Register` (rounded, and
order-sensitive on `LastPracticedAtUtc`) lives in the Repertoire domain —
recomputing it client-side would mean duplicating that arithmetic and
risking drift from whatever the aggregate actually produced. Same
reasoning that keeps the arithmetic out of the consumer itself. A visible
"Refresh" / "Updating…" control on the piece detail page covers the case
where the poll window closes before the consumer finishes.

---

## Verification (RNF04, RNF05)

Checked by inspection, current as of this commit:

- `Cadence.Repertoire.Domain.csproj` and `Cadence.Practice.Domain.csproj`
  contain zero `<PackageReference>` and zero `<ProjectReference>` entries,
  including no reference to `Cadence.Contracts`. Both domains are pure C#,
  compilable in isolation.
- `grep -rn "Status ==" src` outside the two Domain projects returns exactly
  one hit: `PieceRepository.cs`, `query.Where(piece => piece.Status ==
  status)`. That's an EF Core query predicate implementing the `status`
  filter on `GET /pieces` (T29). It filters rows, it doesn't decide
  anything. The rule RNF05 protects, "no code outside `Piece.cs` decides
  whether a status transition is legal," still holds: `ChangeStatus` and
  `RegisterPractice` on `Piece.cs` are the only places that branch on what a
  status transition means. Judgment call, noted here so it doesn't read as
  an oversight.
- `dotnet build Cadence.slnx` succeeds with 0 warnings, 0 errors
  (`TreatWarningsAsErrors=true` from `Directory.Build.props`, so this is a
  real gate, not a formality).

## End-to-end walkthrough (§14 definition of done)

Traced against the code. Run [Cadence.http](Cadence.http) yourself to see it
live (this sandbox has no running Docker daemon, so it's verified by reading
the implementation, not by executing it here):

| Item | Where it's enforced |
|---|---|
| `docker compose up -d` + `dotnet run` × 2, nothing else to install | `docker-compose.yaml`; `Database.Migrate()` in both `Program.cs` files |
| Creating a piece then registering a session updates the piece's record within seconds, no HTTP between services | `SessionEndpoints.CreateSession` → MassTransit publish → `PracticeSessionRegisteredConsumer.Consume` → `Piece.RegisterPractice`. No `HttpClient` anywhere in either Api project. |
| First session moves Backlog → Learning | `Piece.RegisterPractice` in [Piece.cs](src/Repertoire/Cadence.Repertoire.Domain/Piece.cs) |
| `PATCH .../status` to Mastered on a never-practiced piece → 409 with a domain reason | `Piece.ChangeStatus` rejects when `Record.SessionCount == 0`; `ResultExtensions.ToProblem(..., 409)` in `PieceEndpoints.ChangePieceStatus` |
| Repertoire stopped → sessions still register, messages queue; restart drains and the record catches up | Practice never calls Repertoire (RNF01) and publishing is fire-and-forget onto RabbitMQ, which persists the queue; the consumer picks up the backlog on reconnect |
| Republishing the same message changes nothing | `ProcessedMessage` dedup check at the top of `Consume`, before any mutation (T40) |
| Unknown-piece event does not block the queue | `piece is null` branch logs a warning and returns normally, no throw, no retry, no `_error` queue (T41) |

**One item needs a live run, not code-reading, before you'll really believe
it:** the Repertoire-stopped-then-restarted recovery. Do it yourself:

```bash
docker compose up -d
dotnet run --project src/Repertoire/Cadence.Repertoire.Api   # then Ctrl+C to stop it
dotnet run --project src/Practice/Cadence.Practice.Api
# POST /sessions on :5002 a few times while Repertoire is down — each returns 201
# open http://localhost:15672 (admin/admin), Queues tab — see the messages sitting there
dotnet run --project src/Repertoire/Cadence.Repertoire.Api   # restart it
# watch the queue drain in the management UI, then GET /pieces/{id} — Record is caught up
```

Retry and the `_error` queue (RNF08, T63): to see it, publish a message the
consumer can't handle (temporarily throw inside `Consume`, say). MassTransit
retries with an increasing interval (1s, then 2s, per
`retryConfigurator.Incremental(3, ...)` in
[Program.cs](src/Repertoire/Cadence.Repertoire.Api/Program.cs)), and then the
message lands in `practice-session-registered_error` in the management UI.

---

## Strategic design — context map

**Repertoire** is the context of what I'm learning: works, composers,
difficulty, progress toward mastery. It owns `Piece` and is the sole
authority on a piece's status.

**Practice** is the context of what I actually did: sessions, duration,
tempo, focus, how well it went. It owns `PracticeSession` and is the sole
authority on the historical record.

Repertoire is downstream of Practice for the flow of *facts*. Practice
publishes what happened, and Repertoire reacts by folding it into the piece.
Practice is downstream of Repertoire for *identity*: a session refers to a
piece that Repertoire created, via a bare `PieceId` Practice never validates.

**The relationship is conformist in both directions.** Practice accepts
`PieceId` exactly as Repertoire defines it (a `Guid`, taken at face value,
never checked). Repertoire accepts the integration event shape Practice
publishes exactly as published, with no translation layer softening it. In a
system with more than one team you'd put an anti-corruption layer on at
least one side, to stop one context's model changes from leaking into the
other's. Here, with one developer and one repository, that layer would be
pure ceremony. There's no second team whose model drift it would protect
against, and the two shapes (`PieceId`, `PracticeSessionRegistered`) are
already minimal and stable by construction (`Cadence.Contracts` has zero
other dependents). Conformist is the honest choice here, not a shortcut
taken under time pressure.

Practice also keeps its own `PieceReference`, the piece's id plus a
**snapshot** of its title, rather than looking anything up. That's
event-carried state transfer, done on purpose: Practice doesn't need
composer, key, difficulty, or status to render a history list, and a session
record describes what you practiced *at the time*. So title drift after a
rename is accepted. It's not a bug.

---

**Why doesn't `Cadence.Repertoire.Domain` reference `Cadence.Contracts`?
What would actually break if it did?**

The domain models Repertoire's own ubiquitous language, `Piece`,
`LearningStatus`, `PracticeRecord`, and nothing about wire formats.
`Cadence.Contracts` exists to describe what crosses the RabbitMQ boundary,
which is an Api-layer concern (MassTransit consumers live in
`Cadence.Repertoire.Api`, not `.Domain`). If Domain referenced Contracts, two
things would break. First, the dependency rule Api → Domain, Infrastructure,
Contracts / Domain → nothing becomes false, so `T23`'s
zero-package-reference check (and the architecture test it encodes) fails
immediately, and it becomes physically possible to write `PracticeFocus` or
`PracticeSessionRegistered` straight into aggregate logic. The domain-local
`PracticeFocus` enum duplicated in `Practice.Domain` (T45) exists
specifically to prevent that coupling. Second, Contracts is a public wire
contract shared by both services. Changing it to suit one domain's internal
modeling needs would ripple into the other service's deserialization, which
is exactly the kind of coupling bounded contexts are supposed to prevent.

**`PracticeRecord.Register` returns a new value object instead of mutating
the existing one. What does immutability actually buy here?**

Three things, concretely. First, thread and concurrency safety for free:
nothing can observe a `PracticeRecord` in a half-updated state (minutes
summed but count not yet incremented), because there's no such state. The
transition is atomic by construction, not by convention. Second, it makes
`Piece.RegisterPractice`'s job trivially auditable: `Record =
Record.Register(...)` is a single assignment, so EF Core's `ComplexProperty`
change tracking (T27) sees "whole property replaced" rather than needing to
diff four separate mutated fields. The persistence model matches the
domain model's actual semantics. And the real payoff: it makes the
arithmetic in `Register` pure and trivially unit-testable in isolation.
Given inputs, assert on the returned value, no setup or teardown of mutable
state, no ordering-dependent test pollution. A mutable class buys nothing
here in exchange, since nothing about practice history is ever supposed to
be partially visible or reverted.

**The rule "a piece cannot be mastered without practice" lives on the
aggregate. Name two other places it could plausibly have gone, and say what
goes wrong in each.**

*In the API endpoint* (`PieceEndpoints.ChangePieceStatus`, checking
`piece.Record.SessionCount == 0` before calling `ChangeStatus`): now the rule
exists in two places the moment `RegisterPractice`'s automatic-promotion
path also needs to reason about masterability, and the two copies can
silently diverge. That's exactly the anemic-domain-model failure mode where
"the real business logic" leaks into a layer that's supposed to be plumbing.
Any second caller of `ChangeStatus` (a future admin tool, a batch job, a
test harness) gets an *unenforced* rule unless it remembers to re-check.

*In a database constraint* (a `CHECK` on the `Status`/`SessionCount`
columns, say): the rule becomes invisible to anyone reading C#, fires only
at `SaveChanges` time as an opaque SQL error instead of a `Result` with a
readable reason (breaking RNF06/RNF07's non-exceptional failure channel and
the 409 Problem Details contract), and SQLite's `CHECK` support is
constrained enough that expressing "SessionCount == 0" cleanly across a
`ComplexProperty`-mapped value object is awkward at best.

**If a session is registered and RabbitMQ is briefly unreachable, what does
the user actually see, and what state is the system left in? What would the
outbox pattern change about that answer?**

The user sees success. `PracticeDbContext.SaveChangesAsync` already
committed the `PracticeSession` row before the publish is even attempted, so
`POST /sessions` still returns `201 Created` with the session in the
response body. Nothing about the HTTP response reveals that RabbitMQ was
unreachable. What actually happens to the publish depends on where "briefly
unreachable" lands: if MassTransit's bus is still starting up or
reconnecting when `IPublishEndpoint.Publish` is called, the call can throw
(the endpoint isn't ready) or block briefly waiting for a channel. Either
way there's no retry wrapping *that specific call* the way there is on the
consumer side. It either eventually succeeds once the connection recovers,
or the request fails outright, or the event is silently dropped, and
nothing recorded anywhere says "this session's event was supposed to be
published and wasn't." The system is left with a `PracticeSession` in
`practice.db` that Repertoire may never learn about. That's the exact gap
documented in [KNOWN-ISSUES.md](KNOWN-ISSUES.md).

The outbox pattern changes this by moving the publish out of the request
path entirely. `SaveChangesAsync` would write both the `PracticeSession` row
*and* an outbox row (the serialized `PracticeSessionRegistered` payload) in
one SQLite transaction, so either both are durable or neither is. No gap is
possible. A separate background dispatcher then drains the outbox table and
publishes, retrying indefinitely against RabbitMQ until it succeeds, and
deleting the outbox row only after a confirmed publish. The user-visible
behavior gets strictly better in the failure case (the event is guaranteed
to eventually reach Repertoire, no silent loss), at the cost of eventual,
not immediate, consistency between "session saved" and "event published."
This system already tolerates that everywhere else anyway, since RabbitMQ
delivery was never synchronous with the HTTP response to begin with.