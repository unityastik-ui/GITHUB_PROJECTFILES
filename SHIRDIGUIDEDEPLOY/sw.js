const CACHE_NAME = "shirdi-guide-v1";
const APP_SHELL_PATHS = [
  "./",
  "./SmartTempleExplorer.html",
  "./QRFINAL%20VERSION.html",
  "./manifest.json",
  "./pwa-192x192.png",
  "./pwa-512x512.png",
  "./pwa-maskable-512x512.png",
];

async function cacheResponse(cache, request, response) {
  if (!response.ok && response.type !== "opaque") return response;

  try {
    await cache.put(request, response.clone());
  } catch (error) {
    console.warn("Unable to update the offline cache:", error);
  }

  return response;
}

async function fetchAndCache(request, cache) {
  const response = await fetch(request);
  return cacheResponse(cache, request, response);
}

async function findNavigationFallback(cache) {
  const scopeUrl = self.registration.scope;
  const fallbackUrls = [
    scopeUrl,
    new URL("SmartTempleExplorer.html", scopeUrl).href,
    new URL("QRFINAL%20VERSION.html", scopeUrl).href,
  ];

  for (const url of fallbackUrls) {
    const response = await cache.match(url);
    if (response) return response;
  }

  return undefined;
}

self.addEventListener("install", (event) => {
  event.waitUntil(
    (async () => {
      const cache = await caches.open(CACHE_NAME);

      await Promise.all(
        APP_SHELL_PATHS.map(async (path) => {
          const url = new URL(path, self.registration.scope);
          try {
            const response = await fetch(url, { cache: "reload" });
            if (response.ok) await cacheResponse(cache, url.href, response);
          } catch {
            // The navigation request will populate this entry on the first visit.
          }
        }),
      );

      await self.skipWaiting();
    })(),
  );
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    (async () => {
      const cacheNames = await caches.keys();
      await Promise.all(
        cacheNames
          .filter(
            (cacheName) =>
              cacheName.startsWith("shirdi-guide-") &&
              cacheName !== CACHE_NAME,
          )
          .map((cacheName) => caches.delete(cacheName)),
      );
      await self.clients.claim();
    })(),
  );
});

self.addEventListener("fetch", (event) => {
  const request = event.request;
  if (request.method !== "GET") return;

  event.respondWith(
    (async () => {
      const cache = await caches.open(CACHE_NAME);

      if (request.mode === "navigate") {
        try {
          return await fetchAndCache(request, cache);
        } catch {
          const cachedPage = await cache.match(request);
          if (cachedPage) return cachedPage;

          const fallbackPage = await findNavigationFallback(cache);
          if (fallbackPage) return fallbackPage;

          throw new Error("No cached page is available for offline navigation.");
        }
      }

      const cachedResponse = await cache.match(request);
      const networkResponse = fetchAndCache(request, cache);

      if (cachedResponse) {
        event.waitUntil(networkResponse.then(
          () => undefined,
          () => undefined,
        ));
        return cachedResponse;
      }

      return networkResponse;
    })(),
  );
});
