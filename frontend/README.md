# Cadence — Frontend

Essa é a interface do Cadence, meu tracker pessoal de prática de piano/teclado. O backend já existe e roda em dois serviços separados. Esse frontend só consome os dois.

E aqui vai o primeiro ponto que importa entender antes de mexer em qualquer coisa: **são duas APIs, em duas portas diferentes**. Repertoire fica na `5001` (o que estou aprendendo: peças, dificuldade, status). Practice fica na `5002` (o que eu de fato pratiquei: sessões, duração, nota). Elas não conversam entre si por HTTP, só por uma fila do RabbitMQ nos bastidores. Pro frontend isso significa duas base URLs, não uma.

## Stack

- Vite + React 19 + TypeScript
- TanStack Query v5 (cache e fetch de servidor, sem Redux nem Zustand)
- React Router v7
- react-hook-form + zod (formulário e validação)
- Tailwind CSS

Sem autenticação. O backend não tem, então a interface também não precisa.

## Rodando localmente

Precisa das duas APIs do backend rodando primeiro (veja o README na raiz do repositório). Depois:

```bash
npm install
npm run dev
```

Abre em `http://localhost:5173`, porta que o CORS do backend já libera.

## Variáveis de ambiente

Copie `.env.example` para `.env.local` e ajuste se precisar:

```
VITE_REPERTOIRE_URL=http://localhost:5001
VITE_PRACTICE_URL=http://localhost:5002
```

Nenhuma URL fica solta espalhada pelo código. Tudo passa por `src/config.ts`.

## Scripts

| Comando             | O que faz                            |
| ------------------- | ------------------------------------ |
| `npm run dev`       | sobe o servidor de desenvolvimento   |
| `npm run build`     | roda o typecheck e depois builda     |
| `npm run preview`   | serve o build de produção localmente |
| `npm run lint`      | roda o linter                        |
| `npm run typecheck` | só o `tsc --noEmit`, sem buildar     |

## Um comportamento que vale saber de cara

Depois que você registra uma sessão de prática (`POST /sessions`), o registro da peça (`record`, com total de minutos, média de nota etc.) não atualiza na hora. Quem atualiza isso é um consumidor de fila do RabbitMQ do lado do Repertoire, e isso leva um tempinho. A interface não pode simplesmente assumir que, um segundo depois de salvar a sessão, o dado da peça já está fresco. Tem mais detalhes disso em [KNOWN-ISSUES.md](KNOWN-ISSUES.md).

## Onde estamos

Scaffold inicial pronto (T01 a T08 do `TASKS.md`): projeto criado, dependências instaladas, TypeScript em modo estrito, Tailwind configurado, variáveis de ambiente definidas, lint e scripts no lugar. Os tipos de domínio e os clientes de API vêm a seguir.
