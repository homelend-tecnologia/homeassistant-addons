import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200, // Número de usuários virtuais simultâneos
    duration: '60s', // Duração do teste
};

export default function () {
    const devices = [
        'temperatura_media_interna',
        'temperatura_media_externa'
    ];

    // Seleciona um dispositivo aleatório
    const device = devices[Math.floor(Math.random() * devices.length)];
    const url = `http://192.168.180.5:8084/api/history/temperature/mean/${device}`;

    // Faz a requisição HTTP GET
    let res = http.get(url);

    // Verifica se a resposta tem status 200
    check(res, {
        'status é 200': (r) => r.status === 200,
    });

    sleep(1); // Aguarda 1 segundo antes de fazer outra requisição
}
