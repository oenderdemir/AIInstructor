# Angular Uygulamasını Derleme Aşaması
FROM node:latest AS angular-build
WORKDIR /app
COPY VTS-ClientApp/package.json VTS-ClientApp/package-lock.json ./
RUN npm install
COPY VTS-ClientApp/ .


RUN npm run prod

FROM nginx:latest AS angular-nginx
RUN apt-get update && apt-get install -y tzdata \
    && ln -fs /usr/share/zoneinfo/Europe/Istanbul /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata
# Angular build sonuçlarını /usr/share/nginx/html dizinine kopyala
COPY --from=angular-build /app/dist/cav-fe/browser /usr/share/nginx/html
COPY VTS-ClientApp/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
