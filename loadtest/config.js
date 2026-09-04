export const repertoireUrl = __ENV.REPERTOIRE_URL || 'http://localhost:5001';
export const practiceUrl = __ENV.PRACTICE_URL || 'http://localhost:5002';

export const genres = [0, 1, 2, 3, 4, 5, 6, 7];
export const difficulties = [0, 1, 2];
export const learningStatuses = [0, 1, 2, 3, 4];
export const focuses = [0, 1, 2, 3, 4];

export function randomItem(items) {
    return items[Math.floor(Math.random() * items.length)];
}

export function randomPieceTitle() {
    return `Load Test Piece ${Math.floor(Math.random() * 1_000_000)}`;
}
