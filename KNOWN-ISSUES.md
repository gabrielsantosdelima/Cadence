# Known Issues

## Dual-write gap between the SQLite commit and the RabbitMQ publish

**Where:** `Cadence.Practice.Api.Endpoints.SessionEndpoints.CreateSession`
(`src/Practice/Cadence.Practice.Api/Endpoints/SessionEndpoints.cs`), which calls
`PracticeDbContext.SaveChangesAsync` and then, as a separate step, calls
`IPublishEndpoint.Publish` (via `PracticeWasRegisteredHandler`).

**The gap:** these are two independent I/O operations against two different
systems (SQLite, RabbitMQ) with no shared transaction. If the process crashes,
loses power, or the RabbitMQ connection drops in the window between the
`SaveChangesAsync` returning and the `Publish` call completing, the
`PracticeSession` row is durably committed to `practice.db` but the
`PracticeSessionRegistered` event is never published. Repertoire never learns
about that session: the piece's `PracticeRecord` silently falls out of sync
with what Practice thinks happened, and nothing surfaces the discrepancy.

**Why it wasn't fixed:** the correct fix is the transactional outbox pattern —
write the integration event to an `Outbox` table in the *same* SQLite
transaction as the `PracticeSession` insert, then have a separate background
process (or MassTransit's built-in outbox, `AddEntityFrameworkOutbox`) drain
that table and publish reliably, with the publish itself becoming idempotent
against retries. That's real infrastructure work — a new table, a migration,
a background dispatcher or a MassTransit outbox integration — deliberately
out of scope for this build. It was cut consciously, not missed: see the
README's answer to "what would the outbox pattern change" for the full
reasoning and what changes if the outbox is added later.

**Blast radius today:** low. The window is a single in-process publish call
right after a successful save — not a network round trip to another service —
so in practice this only bites on a crash or RabbitMQ outage at that exact
instant. The consumer side is already idempotent (`ProcessedMessage` dedup in
`PracticeSessionRegisteredConsumer`), so if the outbox is added later, nothing
downstream needs to change to tolerate at-least-once delivery — it already
does.
