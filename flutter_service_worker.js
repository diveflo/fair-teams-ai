'use strict';
const MANIFEST = 'flutter-app-manifest';
const TEMP = 'flutter-temp-cache';
const CACHE_NAME = 'flutter-app-cache';
const RESOURCES = {
  "favicon.png": "5dcef449791fa27946b3d35ad8803796",
"icons/hnyb.jpg": "d21a81d1ff5564602416ebd2c206ff8c",
"icons/Icon-512.png": "96e752610906ba2a93c65f8abe1645f1",
"icons/Icon-192.png": "ac9a721a12bbc803b44f645561ecb1e1",
"manifest.json": "f326e6dd256d602d4c6274b186f43d4e",
"version.json": "b0b051455791d0b62d0847bab362cd7d",
"canvaskit/profiling/canvaskit.js": "ae2949af4efc61d28a4a80fffa1db900",
"canvaskit/profiling/canvaskit.wasm": "95e736ab31147d1b2c7b25f11d4c32cd",
"canvaskit/canvaskit.js": "c2b4e5f3d7a3d82aed024e7249a78487",
"canvaskit/canvaskit.wasm": "4b83d89d9fecbea8ca46f2f760c5a9ba",
"main.dart.js": "9c5bfe7459ed61fb94a918adc39564bf",
"index.html": "c0b2a24214ceb2273484c2d2b5198535",
"/": "c0b2a24214ceb2273484c2d2b5198535",
"assets/packages/cupertino_icons/assets/CupertinoIcons.ttf": "6d342eb68f170c97609e9da345464e5e",
"assets/FontManifest.json": "dc3d03800ccca4601324923c0b1d6d57",
"assets/NOTICES": "6f6e59cb6af842b91a847dce8af3b727",
"assets/AssetManifest.json": "854083a8f3e908ad9ede8c528686b55f",
"assets/fonts/MaterialIcons-Regular.otf": "7e7a6cccddf6d7b20012a548461d5d81",
"assets/assets/SilverEliteMaster.png": "f59164254d83874a8ac0a164dbd1adb1",
"assets/assets/SilverElite.png": "f59164254d83874a8ac0a164dbd1adb1",
"assets/assets/MasterGuardianElite.png": "9b0811adecc765ed96198ec33814ef3f",
"assets/assets/LegendaryEagle.png": "5385349d680414f037183af9bc30b5fd",
"assets/assets/overpass.jpg": "140cd30663dbeca0627b97612ab4cdc3",
"assets/assets/DistinguishedMasterGuardian.png": "380100059031fb79ff2489c4d225c649",
"assets/assets/mirage.jpg": "5713a9f3b6da1ce9a6141b6b0f9ecf24",
"assets/assets/ancient_calls.webp": "37bda1d9d964441cbfd610883ae0b565",
"assets/assets/GoldNovaII.png": "19e07ee34e43027ae2eec665ea39b2ff",
"assets/assets/silverIII.png": "7ce6ef71a2885927c4fb146886dea776",
"assets/assets/silverII.png": "409a71ef7daf1e7d544ba6902b4fea80",
"assets/assets/TheGlobalElite.png": "baa1c00532696a7d66289d230204161d",
"assets/assets/nuke_calls.jpeg": "f9dccd1b5dcb5f4545c45a186921dc09",
"assets/assets/GoldNovaIII.png": "7f20bc4fb38e892baf9c02f0f16171c0",
"assets/assets/overpass_calls.jpeg": "610ab32728247ea5f2f949639a9da2c0",
"assets/assets/MasterGuardianI.png": "da20a3d259c71e48831e3e004a50da5a",
"assets/assets/ancient.jpg": "378f80dda0d16c44a91551bf04499e26",
"assets/assets/nuke.jpg": "5edcad932b6eb9f747ac114a13185469",
"assets/assets/vertigo_calls.jpeg": "bf7259034e256151122891b53f850bd1",
"assets/assets/MasterGuardianII.png": "5e13186fb191d3e27d26e528a8a66e22",
"assets/assets/t.png": "c31727b5657f8a1bb65e833903fc2838",
"assets/assets/train.jpg": "0eb5f2de57c081ded0d0e4865e3e24af",
"assets/assets/GoldNovaI.png": "4ccf5df1cba5dbb20ffe38a79db978bd",
"assets/assets/cs.jpg": "649968782a00db43e309ec619966aa34",
"assets/assets/mirage_calls.jpeg": "d60197bd24a1765b062bbf610ceef55e",
"assets/assets/silverI.png": "1834a8591b002e3692f8d1cf6ad0e156",
"assets/assets/hnyb.jpg": "d21a81d1ff5564602416ebd2c206ff8c",
"assets/assets/vertigo.jpg": "dcd8cbbb3089415b1dbc1530600b0ab3",
"assets/assets/LegendaryEagleMaster.png": "d812c375d5124c0ff4ca921d1526f8b8",
"assets/assets/inferno.jpg": "04db9541610523d7a1fb724d5778a6eb",
"assets/assets/SupremeMasterFirstClass.png": "e8f14da59c5fa66312d731b87df3441a",
"assets/assets/inferno_calls.jpeg": "8a00105df97c626df46cda88ea537497",
"assets/assets/GoldNovaMaster.png": "3d9d1638777c7e21b834a0cc139e6e60",
"assets/assets/dust2_calls.jpeg": "77c2e9a4b3d831abf8e52a55ca62ee4b",
"assets/assets/SilverIV.png": "1478d612c64b939ab14031225b095bc4",
"assets/assets/dust2.jpg": "5ba7722282e87c0cbc1145fd383d8c49",
"assets/assets/ct.png": "8881d1ff23b156e9b0ea03c5f85166c4",
"assets/assets/NotRanked.png": "c02dad2fe545f57ada8e5b4605371e02"
};

// The application shell files that are downloaded before a service worker can
// start.
const CORE = [
  "/",
"main.dart.js",
"index.html",
"assets/NOTICES",
"assets/AssetManifest.json",
"assets/FontManifest.json"];
// During install, the TEMP cache is populated with the application shell files.
self.addEventListener("install", (event) => {
  self.skipWaiting();
  return event.waitUntil(
    caches.open(TEMP).then((cache) => {
      return cache.addAll(
        CORE.map((value) => new Request(value, {'cache': 'reload'})));
    })
  );
});

// During activate, the cache is populated with the temp files downloaded in
// install. If this service worker is upgrading from one with a saved
// MANIFEST, then use this to retain unchanged resource files.
self.addEventListener("activate", function(event) {
  return event.waitUntil(async function() {
    try {
      var contentCache = await caches.open(CACHE_NAME);
      var tempCache = await caches.open(TEMP);
      var manifestCache = await caches.open(MANIFEST);
      var manifest = await manifestCache.match('manifest');
      // When there is no prior manifest, clear the entire cache.
      if (!manifest) {
        await caches.delete(CACHE_NAME);
        contentCache = await caches.open(CACHE_NAME);
        for (var request of await tempCache.keys()) {
          var response = await tempCache.match(request);
          await contentCache.put(request, response);
        }
        await caches.delete(TEMP);
        // Save the manifest to make future upgrades efficient.
        await manifestCache.put('manifest', new Response(JSON.stringify(RESOURCES)));
        return;
      }
      var oldManifest = await manifest.json();
      var origin = self.location.origin;
      for (var request of await contentCache.keys()) {
        var key = request.url.substring(origin.length + 1);
        if (key == "") {
          key = "/";
        }
        // If a resource from the old manifest is not in the new cache, or if
        // the MD5 sum has changed, delete it. Otherwise the resource is left
        // in the cache and can be reused by the new service worker.
        if (!RESOURCES[key] || RESOURCES[key] != oldManifest[key]) {
          await contentCache.delete(request);
        }
      }
      // Populate the cache with the app shell TEMP files, potentially overwriting
      // cache files preserved above.
      for (var request of await tempCache.keys()) {
        var response = await tempCache.match(request);
        await contentCache.put(request, response);
      }
      await caches.delete(TEMP);
      // Save the manifest to make future upgrades efficient.
      await manifestCache.put('manifest', new Response(JSON.stringify(RESOURCES)));
      return;
    } catch (err) {
      // On an unhandled exception the state of the cache cannot be guaranteed.
      console.error('Failed to upgrade service worker: ' + err);
      await caches.delete(CACHE_NAME);
      await caches.delete(TEMP);
      await caches.delete(MANIFEST);
    }
  }());
});

// The fetch handler redirects requests for RESOURCE files to the service
// worker cache.
self.addEventListener("fetch", (event) => {
  if (event.request.method !== 'GET') {
    return;
  }
  var origin = self.location.origin;
  var key = event.request.url.substring(origin.length + 1);
  // Redirect URLs to the index.html
  if (key.indexOf('?v=') != -1) {
    key = key.split('?v=')[0];
  }
  if (event.request.url == origin || event.request.url.startsWith(origin + '/#') || key == '') {
    key = '/';
  }
  // If the URL is not the RESOURCE list then return to signal that the
  // browser should take over.
  if (!RESOURCES[key]) {
    return;
  }
  // If the URL is the index.html, perform an online-first request.
  if (key == '/') {
    return onlineFirst(event);
  }
  event.respondWith(caches.open(CACHE_NAME)
    .then((cache) =>  {
      return cache.match(event.request).then((response) => {
        // Either respond with the cached resource, or perform a fetch and
        // lazily populate the cache.
        return response || fetch(event.request).then((response) => {
          cache.put(event.request, response.clone());
          return response;
        });
      })
    })
  );
});

self.addEventListener('message', (event) => {
  // SkipWaiting can be used to immediately activate a waiting service worker.
  // This will also require a page refresh triggered by the main worker.
  if (event.data === 'skipWaiting') {
    self.skipWaiting();
    return;
  }
  if (event.data === 'downloadOffline') {
    downloadOffline();
    return;
  }
});

// Download offline will check the RESOURCES for all files not in the cache
// and populate them.
async function downloadOffline() {
  var resources = [];
  var contentCache = await caches.open(CACHE_NAME);
  var currentContent = {};
  for (var request of await contentCache.keys()) {
    var key = request.url.substring(origin.length + 1);
    if (key == "") {
      key = "/";
    }
    currentContent[key] = true;
  }
  for (var resourceKey of Object.keys(RESOURCES)) {
    if (!currentContent[resourceKey]) {
      resources.push(resourceKey);
    }
  }
  return contentCache.addAll(resources);
}

// Attempt to download the resource online before falling back to
// the offline cache.
function onlineFirst(event) {
  return event.respondWith(
    fetch(event.request).then((response) => {
      return caches.open(CACHE_NAME).then((cache) => {
        cache.put(event.request, response.clone());
        return response;
      });
    }).catch((error) => {
      return caches.open(CACHE_NAME).then((cache) => {
        return cache.match(event.request).then((response) => {
          if (response != null) {
            return response;
          }
          throw error;
        });
      });
    })
  );
}
