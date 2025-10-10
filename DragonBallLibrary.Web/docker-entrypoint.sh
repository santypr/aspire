#!/bin/sh

echo "=== DragonBall Web Application Starting ==="

# Quick environment check (only show if debug mode)
if [ "$REACT_APP_ENVIRONMENT" = "development" ]; then
    echo "Environment Variables:"
    echo "  NODE_ENV: $NODE_ENV"
    echo "  REACT_APP_ENVIRONMENT: $REACT_APP_ENVIRONMENT" 
    echo "  REACT_APP_API_URL: $REACT_APP_API_URL"
    echo "  Aspire HTTPS: $services__apiservice__https__0"
    echo "  Aspire HTTP: $services__apiservice__http__0"
fi

echo "=== Configuring Environment ==="

# Determine environment and API URL from various sources
# Priority: Aspire service discovery -> explicit env vars -> defaults
ENVIRONMENT="${REACT_APP_ENVIRONMENT:-${NODE_ENV:-development}}"

# .NET Aspire service discovery variables
# Format: services__<servicename>__<scheme>__<endpoint_index>
API_URL=""
if [ -n "$services__apiservice__https__0" ]; then
    API_URL="$services__apiservice__https__0"
    echo "Using Aspire HTTPS service discovery: $API_URL"
elif [ -n "$services__apiservice__http__0" ]; then
    API_URL="$services__apiservice__http__0"
    echo "Using Aspire HTTP service discovery: $API_URL"
elif [ -n "$REACT_APP_API_URL" ]; then
    API_URL="$REACT_APP_API_URL"
    echo "Using explicit API URL: $API_URL"
else
    API_URL="http://localhost:5304"
    echo "Using default API URL: $API_URL"
fi

echo "Final configuration:"
echo "  Environment: $ENVIRONMENT"
echo "  API URL: $API_URL"

# Generate nginx configuration dynamically
echo "Generating nginx configuration..."
sed "s|API_PLACEHOLDER|$API_URL|g" /etc/nginx/conf.d/default.conf > /etc/nginx/conf.d/default.conf.tmp
mv /etc/nginx/conf.d/default.conf.tmp /etc/nginx/conf.d/default.conf

echo "Debug: Generated nginx config with API URL: $API_URL"

# Generate runtime env-config.js from environment variables
echo "Generating env-config.js..."
cat > /usr/share/nginx/html/env-config.js << EOF
(function (window) {
  window.__env = window.__env || {};
  
  // Environment configuration
  window.__env.REACT_APP_API_URL = "$API_URL";
  window.__env.REACT_APP_ENVIRONMENT = "$ENVIRONMENT";
})(this);
EOF

echo "Environment configuration completed successfully"

# Test nginx configuration
echo "=== Testing Nginx Configuration ==="
nginx -t
if [ $? -ne 0 ]; then
    echo "ERROR: Nginx configuration test failed"
    exit 1
fi

# Start nginx
echo "=== Starting Nginx ==="
echo "Serving application on port 80..."

# Add debug info
echo "Debug: About to start nginx with daemon off"
echo "Debug: Current directory: $(pwd)"
echo "Debug: Files in /usr/share/nginx/html:"
ls -la /usr/share/nginx/html/ | head -10

# Start nginx in foreground
exec nginx -g "daemon off;"