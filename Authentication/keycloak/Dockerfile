FROM quay.io/keycloak/keycloak:latest

# Copiar o arquivo do realm para o contêiner
COPY ./config/realm-export.json /opt/keycloak/data/import/realm-export.json

# Definir variáveis de ambiente
ENV KC_DB=postgres
ENV KC_DB_URL=jdbc:postgresql://keycloakdb.c3y26so8qgto.us-east-2.rds.amazonaws.com:5432/keycloakdb
ENV KC_DB_USERNAME=keycloak
ENV KC_DB_PASSWORD=A7wY8jK2!
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true
ENV KEYCLOAK_ADMIN=admin
ENV KEYCLOAK_ADMIN_PASSWORD=A7wY8jK2

# Expor portas necessárias
EXPOSE 8080 8443

# Iniciar o Keycloak e atualizar a configuração do realm
CMD ["start-dev", "--import-realm"]