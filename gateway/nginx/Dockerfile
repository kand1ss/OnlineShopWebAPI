FROM nginx:alpine

COPY gateway/nginx/nginx.conf /etc/nginx/nginx.conf
COPY gateway/nginx/certs /etc/nginx/certs

CMD ["nginx", "-g", "daemon off;"]