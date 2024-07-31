'use strict';
var port = process.env.PORT || 8080;

const express = require('express');
const cors = require('cors');
const app = express();
const bodyParser = require('body-parser');

app.use(bodyParser.json());
app.use(bodyParser.text());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.raw());

app.use(cors());

const main = () => {
    app.all('/*', getInfos);

    app.listen(port, () => {
        console.log('Listening on port ' + port);
    });
};

const getInfos = (req, res) => {
    const remoteIp = req.headers['x-forwarded-for'] || req.connection.remoteAddress;

    const infos = {
        remoteIp: remoteIp,
        method: req.method,
        url: req.url,
        _parsedUrl: req._parsedUrl,
        query: req.query,
        params: req.params,
        headers: req.headers,
        body: req.body,
        environment: process.env
    };

    console.log(infos);

    res.send(infos);
};

main();