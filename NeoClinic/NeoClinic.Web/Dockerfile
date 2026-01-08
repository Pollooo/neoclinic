# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --legacy-peer-deps

# Copy source code
COPY . .

# Build the Angular app for production
RUN npm run build -- --configuration=production

# Production stage
FROM node:20-alpine
WORKDIR /app

# Copy built application
COPY --from=build /app/dist/NeoClinic.Web ./dist/NeoClinic.Web

# Install only express (needed for SSR)
RUN npm install express@^5.1.0

# Expose port
EXPOSE 8080

# Set environment variables
ENV PORT=8080
ENV NODE_ENV=production

# Start the server
CMD ["node", "dist/NeoClinic.Web/server/server.mjs"]

