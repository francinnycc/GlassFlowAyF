(() => {
 const v = document.getElementById('visorAR');
 const status = document.getElementById('estadoAR');
 const usage = document.getElementById('usoAR');
 const ids = ['forma', 'acabado', 'vidrio'];
 if (!v || !status || !usage) return;
 const base = new URL(v.getAttribute("src"), location.href);
 const say = () => {
   status.textContent = !v.loaded ? 'Cargando modelo...' : v.canActivateAR ? 'Modelo listo. Tocá Ver en mi espacio para iniciar AR.' : 'Modelo listo. Podés explorar en 3D. AR requiere un dispositivo compatible y HTTPS confiable.';
 };
 function update() {
   usage.value = 'false';
   status.textContent = 'Cargando modelo...';
   const url = new URL(base);
   ids.forEach(id => url.searchParams.set(id, document.getElementById(id).value));
   v.src = url.href;
 }
 ids.forEach(id => document.getElementById(id).addEventListener('change', update));
 document.querySelectorAll('[data-orbit]').forEach(b => b.addEventListener('click', () => { v.cameraOrbit = b.dataset.orbit; }));
 v.addEventListener('load', () => { say(); setTimeout(say, 700); });
 v.addEventListener('error', () => { status.textContent = 'No se pudo cargar el modelo. Revisá la conexión y los archivos GLB.'; });
 v.addEventListener('ar-status', e => {
   if (['session-started', 'object-placed'].includes(e.detail.status)) usage.value = 'true';
   if (e.detail.status === 'failed') status.textContent = 'No se pudo iniciar AR. Seguí con la vista 3D o probá Safari en iPhone o Chrome en Android.';
 });
 v.querySelector('[slot="ar-button"]').addEventListener('click', () => {
   status.textContent = 'Abriendo AR. Las sesiones nativas externas no se pueden confirmar automáticamente desde esta página.';
 });
 customElements.whenDefined('model-viewer').then(say);
 setTimeout(() => { if (!customElements.get('model-viewer')) status.textContent = 'No se pudo cargar el visor externo. Revisá internet.'; }, 10000);
})();
