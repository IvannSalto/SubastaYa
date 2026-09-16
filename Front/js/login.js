document.addEventListener('DOMContentLoaded', () => {
  const loginModal = document.getElementById('loginModal');
  const openLoginBtn = document.getElementById('openLoginBtn');
  const closeLoginBtn = document.getElementById('closeLoginBtn');

  // Vistas
  const loginView = document.getElementById('loginView');
  const registerView = document.getElementById('registerView');

  // Botones de alternancia de vista
  const goToRegisterBtn = document.getElementById('goToRegister');
  const goToLoginBtn = document.getElementById('goToLogin');

  // Formularios
  const loginForm = document.getElementById('loginForm');
  const registerForm = document.getElementById('registerForm');

  // Abrir modal al presionar Iniciar Sesión en la barra superior
  if (openLoginBtn && loginModal) {
    openLoginBtn.addEventListener('click', (e) => {
      e.preventDefault();
      loginModal.classList.add('active');
    });
  }

  // Función para cerrar modal y reiniciar vista
  const closeModal = () => {
    if (loginModal) {
      loginModal.classList.remove('active');
      setTimeout(() => {
        loginView.classList.remove('hidden');
        registerView.classList.add('hidden');
      }, 300);
    }
  };

  if (closeLoginBtn) closeLoginBtn.addEventListener('click', closeModal);

  if (loginModal) {
    loginModal.addEventListener('click', (e) => {
      if (e.target === loginModal) closeModal();
    });
  }

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && loginModal && loginModal.classList.contains('active')) {
      closeModal();
    }
  });

  // Cambiar a pantalla de Registro
  if (goToRegisterBtn) {
    goToRegisterBtn.addEventListener('click', () => {
      loginView.classList.add('hidden');
      registerView.classList.remove('hidden');
    });
  }

  // Cambiar a pantalla de Login
  if (goToLoginBtn) {
    goToLoginBtn.addEventListener('click', () => {
      registerView.classList.add('hidden');
      loginView.classList.remove('hidden');
    });
  }

  // Enviar Formulario de Registro
  if (registerForm) {
    registerForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const username = document.getElementById('regUser').value;
      alert(`¡Cuenta creada con éxito para ${username}!`);
      registerForm.reset();
      closeModal();
    });
  }

  // Enviar Formulario de Login
  if (loginForm) {
    loginForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const username = document.getElementById('username').value;
      alert(`¡Bienvenido de nuevo, ${username}!`);
      loginForm.reset();
      closeModal();
    });
  }
});