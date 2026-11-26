// Módulo 15 – Revisión y Aprobación del Cliente
// La validación del servidor es la autoridad; este script solo mejora la experiencia.
(function () {
    'use strict';

    var MINIMO = 10;

    // Aprobación: botón habilitado solo al aceptar términos y condiciones (escenario 2.2).
    document.querySelectorAll('#aceptoTerminos').forEach(function (check) {
        var boton = check.closest('form').querySelector('.js-aprobar');
        check.addEventListener('change', function () { boton.disabled = !check.checked; });
    });

    // Modificación / rechazo: comentario obligatorio (escenario 3.3).
    document.querySelectorAll('.js-requiere-comentario').forEach(function (form) {
        var area = form.querySelector('textarea[name="comentario"]');
        var alerta = form.querySelector('.js-alerta-comentario');

        function invalido() { return area.value.trim().length < MINIMO; }

        form.addEventListener('submit', function (e) {
            if (invalido()) {
                e.preventDefault();
                e.stopImmediatePropagation();
                area.classList.add('is-invalid');
                alerta.classList.remove('d-none');
                area.focus();
            }
        });

        area.addEventListener('input', function () {
            if (!invalido()) {
                area.classList.remove('is-invalid');
                alerta.classList.add('d-none');
            }
        });
    });

    // Evita envíos dobles (doble clic) – el servidor además rechaza versiones repetidas.
    document.querySelectorAll('.js-form-revision').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (e.defaultPrevented) return;
            form.querySelectorAll('button[type="submit"]').forEach(function (b) {
                b.disabled = true;
                b.insertAdjacentHTML('afterbegin', '<span class="spinner-border spinner-border-sm me-1"></span>');
            });
        });
    });
})();
