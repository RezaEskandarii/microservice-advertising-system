import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 300, 
  iterations: 1000, 
};

export default function () {
  const url = 'http://127.0.0.1:5005/api/v1/deposit';
  
  const headers = {
    'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYwNmY5YThhLTYwN2ItNDFkMy04MmNjLTBiOGRlZDhiNDkwYyIsImVtYWlsIjoidXNlckBleGFtcGxlLmNvbSIsImZpcnN0TmFtZSI6InJlemEiLCJsYXN0TmFtZSI6ImVza2FuZGFyaSIsIm5iZiI6OTY5MzM3OTcyOCwiZXhwIjo5NjkzMzgwNjI4LCJpYXQiOjE2OTMzNzk3Mjh9.H_Dxg08yXIXUuUVjHekKwhF59ZJNJo28Wgl3pjen3hI',
    'X-Idempotency-Key': 'ee10009d-27a8-4252-b536-7d9f7fadba30',
    'Content-Type': 'application/json',
  };

  const payload = JSON.stringify({
    Amount: 3000
  });

  const res = http.post(url, payload, { headers });

  console.log('🟢 Status is: '+ res.status);
  console.log('🟢 Body   is: '+ res.body);
  	
  check(res, {
    '🟢 Status is 200': (r) => r.status === 200,
    '⚠️ Response time < 500ms': (r) => r.timings.duration < 500,
  });

  sleep(1);
}
