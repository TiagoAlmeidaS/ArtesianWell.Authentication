FROM bitnami/keycloak:latest

COPY ./config/realm-export.json /opt/bitnami/keycloak/data/import/realm-export.json

ENV KEYCLOAK_ADMIN_USER=admin
ENV KEYCLOAK_ADMIN_PASSWORD=$2a$10$e4rFtJek4rueJe3RFnkOeOb5uF6v3SkOv/Jf6A5ueQ.yt7jrG7QG2
ENV KEYCLOAK_DATABASE_HOST=keycloakdb.c3y26so8qgto.us-east-2.rds.amazonaws.com
ENV KEYCLOAK_DATABASE_PORT=5432
ENV KEYCLOAK_DATABASE_NAME=keycloakdb
ENV KEYCLOAK_DATABASE_USER=keycloak
ENV KEYCLOAK_DATABASE_PASSWORD=A7wY8jK2!

EXPOSE 8080
