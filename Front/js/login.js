const loginModal = document.getElementById('loginModal');
  const openLoginBtn = document.getElementById('openLoginBtn');
  const closeLoginBtn = document.getElementById('closeLoginBtn');

  // Abrir modal al hacer clic en Iniciar Sesión
  if (openLoginBtn) {
    openLoginBtn.addEventListener('click', (e) => {
      e.preventDefault();
      loginModal.classList.add('active');
    });
  }

  // Cerrar modal al hacer clic en la X
  closeLoginBtn.addEventListener('click', () => {
    loginModal.classList.remove('active');
  });

  // Cerrar modal al hacer clic fuera del recuadro
  loginModal.addEventListener('click', (e) => {
    if (e.target === loginModal) {
      loginModal.classList.remove('active');
    }
  });