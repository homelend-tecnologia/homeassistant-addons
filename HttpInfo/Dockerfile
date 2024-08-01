FROM node:lts-alpine
#FROM arm64v8/node:lts-alpine
#FROM --platform=$BUILDPLATFORM node:lts-alpine

ENV HOME=/home/app
ENV NODE_ENV=production
ENV PORT=8083

WORKDIR $HOME/library

COPY package.json .

RUN cat package.json

RUN npm install --production

COPY . .

EXPOSE $PORT

# Usar um script de inicialização personalizado
COPY run.sh /run.sh
RUN chmod +x /run.sh

CMD ["/run.sh"]

# CMD ["npm", "start"]
