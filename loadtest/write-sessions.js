import http from 'k6/http';
import { check } from 'k6';
import { repertoireUrl, practiceUrl, genres, difficulties, focuses, randomItem, randomPieceTitle } from './config.js';

export const options = {
    vus: Number(__ENV.VUS || 20),
    duration: __ENV.DURATION || '30s',
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<800'],
    },
};

const PIECE_COUNT = Number(__ENV.PIECE_COUNT || 20);

export function setup() {
    const pieces = [];

    for (let index = 0; index < PIECE_COUNT; index += 1) {
        const title = randomPieceTitle();
        const response = http.post(
            `${repertoireUrl}/pieces`,
            JSON.stringify({
                title,
                composer: 'Load Test Composer',
                genre: randomItem(genres),
                difficulty: randomItem(difficulties),
                key: null,
                referenceUrl: null,
            }),
            { headers: { 'Content-Type': 'application/json' } },
        );

        if (response.status !== 201) {
            throw new Error(`Failed to seed piece: ${response.status} ${response.body}`);
        }

        const body = JSON.parse(response.body);
        pieces.push({ pieceId: body.id, pieceTitle: title });
    }

    return { pieces };
}

export default function (data) {
    const piece = randomItem(data.pieces);
    const startedAtUtc = new Date(Date.now() - 5 * 60 * 1000).toISOString();

    const response = http.post(
        `${practiceUrl}/sessions`,
        JSON.stringify({
            pieceId: piece.pieceId,
            pieceTitle: piece.pieceTitle,
            startedAtUtc,
            durationMinutes: 10 + Math.floor(Math.random() * 50),
            tempoBpm: 60 + Math.floor(Math.random() * 140),
            focus: randomItem(focuses),
            quality: 1 + Math.floor(Math.random() * 5),
            notes: null,
        }),
        { headers: { 'Content-Type': 'application/json' } },
    );

    check(response, {
        'status is 201': (result) => result.status === 201,
    });
}
