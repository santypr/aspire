FROM node:20 as build

WORKDIR /app

COPY package.json package.json
COPY package-lock.json package-lock.json

RUN npm install

COPY . .

RUN npm run build

FROM nginx:alpine

# Install curl for health checks
RUN apk add --no-cache curl

# Copy nginx configuration and static files
COPY --from=build /app/default.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

# Copy our custom entrypoint with integrated environment configuration
COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh

# Make script executable
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Expose the default nginx port
EXPOSE 80
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]