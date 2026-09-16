document.addEventListener('DOMContentLoaded', () => {
  const modal = document.getElementById('login-modal');
  const btnOpen = document.getElementById('open-modal-btn');
  const btnClose = document.getElementById('close-modal-btn');

  // Abrir el modal sobre el index sin navegar a otra página
  btnOpen.addEventListener('click', () => {
    modal.classList.add('active');
  });

  // Cerrar al hacer clic en la 'X'
  btnClose.addEventListener('click', () => {
    modal.classList.remove('active');
  });

  // Cerrar al hacer clic en el fondo oscuro fuera de la tarjeta
  modal.addEventListener('click', (event) => {
    if (event.target === modal) {
      modal.classList.remove('active');
    }
  });

  // Cerrar con la tecla ESC
  document.addEventListener('keydown', (event) => {
    if (event.key === 'Escape' && modal.classList.contains('active')) {
      modal.classList.remove('active');
    }
  });
});
