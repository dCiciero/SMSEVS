import http from 'k6/http';
import { check } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 1000 }, // Ramp-up to 1000 users
        { duration: '5m', target: 1000 },  // Sustain 1000 users for 5 minutes
        { duration: '30s', target: 0 },    // Ramp-down
    ],
};

export default function () {
    const url = 'http://localhost:5297/api/sms/simulate'; // Replace with your actual URL
    
    const nationalId = `ID${__VU}${__ITER}`; // Unique NationalId based on Virtual User ID and Iteration
    const name = `User${__VU}${__ITER}`; // Unique Name based on Virtual User ID and Iteration

    const messageContent = `REGISTER ${nationalId} ${name}`;
  
    // Payload for the API request
    const payload = JSON.stringify({
      From: `+2348647${__VU}${__ITER}`, // Use a unique phone number for each virtual user
      Body: messageContent,
    });

    const params = {
        headers: { 'Content-Type': 'application/json' },
    };

    const res = http.post(url, payload, params);

    check(res, {
        'is status 200': (r) => r.status === 200,
    });
}