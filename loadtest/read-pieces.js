import http from 'k6/http';
import { check } from 'k6';
import { repertoireUrl, genres, learningStatuses, randomItem } from './config.js';

export const options = {
    vus: Number(__ENV.VUS || 20),
    duration: __ENV.DURATION || '30s',
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<500'],
    },
};

export default function () {
    const useFilters = Math.random() < 0.5;
    const query = useFilters
        ? `?status=${randomItem(learningStatuses)}&genre=${randomItem(genres)}`
        : '';

    const response = http.get(`${repertoireUrl}/pieces${query}`);

    check(response, {
        'status is 200': (result) => result.status === 200,
    });
}
