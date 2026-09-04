import http from 'k6/http';
import { check } from 'k6';
import { repertoireUrl } from './config.js';

export const options = {
    scenarios: {
        spike: {
            executor: 'ramping-arrival-rate',
            startRate: 10,
            timeUnit: '1s',
            preAllocatedVUs: 50,
            maxVUs: 500,
            stages: [
                { target: 50, duration: '30s' },
                { target: 150, duration: '30s' },
                { target: 300, duration: '30s' },
                { target: 500, duration: '30s' },
                { target: 500, duration: '30s' },
            ],
        },
    },
    thresholds: {
        http_req_failed: ['rate<1'],
    },
};

export default function () {
    const response = http.get(`${repertoireUrl}/pieces`);

    check(response, {
        'status is 200': (result) => result.status === 200,
    });
}
