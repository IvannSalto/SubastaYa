document.addEventListener('DOMContentLoaded', () => {
  const loginModal = document.getElementById('loginModal');
  const openLoginBtn = document.getElementById('openLoginBtn');
  const closeLoginBtn = document.getElementById('closeLoginBtn');

  const loginView = document.getElementById('loginView');
  const registerView = document.getElementById('registerView');
  const goToRegisterBtn = document.getElementById('goToRegister');
  const goToLoginBtn = document.getElementById('goToLogin');
  const loginForm = document.getElementById('loginForm');
  const registerForm = document.getElementById('registerForm');

  const toast = document.getElementById('toast');
  const userDropdown = document.getElementById('userDropdown');
  const userMenuBtn = document.getElementById('userMenuBtn');
  const userDisplayName = document.getElementById('userDisplayName');
  const logoutBtn = document.getElementById('logoutBtn');

  // --- NOTIFICACIONES TOAST ---
  const showToast = (message, isError = false) => {
    if (!toast) return;
    toast.textContent = message;

    if (isError) {
      toast.classList.add('error');
    } else {
      toast.classList.remove('error');
    }

    toast.classList.add('show');

    setTimeout(() => {
      toast.classList.remove('show');
    }, 3500);
  };

  // --- PERSISTENCIA DE SESIÓN ---
  const saveSession = (userData, token) => {
    localStorage.setItem('currentUser', JSON.stringify(userData));
    if (token) {
      localStorage.setItem('token', token); 
    }
    updateUI();
  };

  const clearSession = () => {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    updateUI();
  };

  const getSession = () => {
    const session = localStorage.getItem('currentUser');
    return session ? JSON.parse(session) : null;
  };

  const updateUI = () => {
    const user = getSession();
    const openCreateModalBtn = document.getElementById('openCreateModalBtn');
    if (user) {
      if (openLoginBtn) openLoginBtn.classList.add('hidden');

      if (openCreateModalBtn) {
        openCreateModalBtn.classList.remove('hidden');
        openCreateModalBtn.style.display = 'inline-block'; 
      }

      if (userDropdown) {
        userDropdown.classList.remove('hidden');
        userDisplayName.textContent = user.name || user.email;
      }
    } else {
      if (openLoginBtn) openLoginBtn.classList.remove('hidden');
      if (openCreateModalBtn) {
        openCreateModalBtn.classList.add('hidden');
        openCreateModalBtn.style.display = 'none';
      }
      if (userDropdown) {
        userDropdown.classList.add('hidden');
        userDropdown.classList.remove('open');
      }
    }
  };

  updateUI();

  // --- MENÚ DESPLEGABLE USUARIO ---
  if (userMenuBtn && userDropdown) {
    userMenuBtn.addEventListener('click', (e) => {
      e.stopPropagation();
      userDropdown.classList.toggle('open');
    });

    document.addEventListener('click', (e) => {
      if (!userDropdown.contains(e.target)) {
        userDropdown.classList.remove('open');
      }
    });
  }

  if (logoutBtn) {
    logoutBtn.addEventListener('click', () => {
      clearSession();
      showToast('Sesión cerrada correctamente');
    });
  }

  // --- MODAL DE AUTENTICACIÓN ---
  if (openLoginBtn && loginModal) {
    openLoginBtn.addEventListener('click', (e) => {
      e.preventDefault();
      loginModal.classList.add('active');
    });
  }

  const closeAuthModal = () => {
    if (loginModal) {
      loginModal.classList.remove('active');
      setTimeout(() => {
        loginView.classList.remove('hidden');
        registerView.classList.add('hidden');
      }, 300);
    }
  };

  if (closeLoginBtn) closeLoginBtn.addEventListener('click', closeAuthModal);

  if (goToRegisterBtn) {
    goToRegisterBtn.addEventListener('click', () => {
      loginView.classList.add('hidden');
      registerView.classList.remove('hidden');
    });
  }

  if (goToLoginBtn) {
    goToLoginBtn.addEventListener('click', () => {
      registerView.classList.add('hidden');
      loginView.classList.remove('hidden');
    });
  }

  // Cerrar modal con clic fuera o tecla ESC
  window.addEventListener('click', (e) => {
    if (e.target === loginModal) closeAuthModal();
  });

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
      if (loginModal && loginModal.classList.contains('active')) closeAuthModal();
    }
  });

  // --- FORMULARIO DE REGISTRO ---
  if (registerForm) {
    const passwordInput = document.getElementById('regPassword');
    const confirmInput = document.getElementById('regConfirmPassword');
    const errorElement = document.getElementById('regPasswordError');

    if (confirmInput && errorElement) {
      confirmInput.addEventListener('input', () => {
        errorElement.textContent = '';
        errorElement.classList.remove('visible');
        confirmInput.classList.remove('input-error');
      });
    }

    registerForm.addEventListener('submit', async (e) => {
      e.preventDefault();

      const fullName = document.getElementById('regFullName').value.trim();
      const email = document.getElementById('regEmail').value.trim();
      const password = passwordInput.value;
      const confirmPassword = confirmInput ? confirmInput.value : password;

      if (password !== confirmPassword) {
        showToast('Las contraseñas no coinciden', true);
        if (errorElement && confirmInput) {
          errorElement.textContent = 'Las contraseñas no coinciden.';
          errorElement.classList.add('visible');
          confirmInput.classList.add('input-error');
          confirmInput.focus();
        }
        return;
      }

      try {
        const response = await fetch('http://localhost:5142/api/Auth/register', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ 
            name: fullName, 
            email: email, 
            password: password 
          })
        });

        const result = await response.json().catch(() => ({}));

        if (response.ok) {
          showToast(`¡Cuenta creada con éxito para ${fullName}! Ya puedes iniciar sesión.`);
          registerForm.reset();
          closeAuthModal();
          
          if (loginView && registerView) {
            registerView.classList.add('hidden');
            loginView.classList.remove('hidden');
          }
        } else {
          showToast(result.message || 'Error al registrar el usuario', true);
        }

      } catch (error) {
        console.error('Error de red en el registro:', error);
        showToast('No se pudo conectar con el servidor para registrarse', true);
      }
    });
  }

  // --- FORMULARIO DE INICIO DE SESIÓN (LOGIN) ---
  if (loginForm) {
    loginForm.addEventListener('submit', async (e) => {
      e.preventDefault();

      const email = document.getElementById('email').value.trim();
      const password = document.getElementById('password').value;

      try {
        const response = await fetch('http://localhost:5142/api/Auth/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, password })
        });

        const result = await response.json().catch(() => ({}));

        if (response.ok) {  
          let token = null;
          if (typeof result === 'string') {
              token = result;
          } else if (result) {
              const rawToken = result.token || result.accessToken || result.data;
              
              if (typeof rawToken === 'object' && rawToken !== null) {
                  token = rawToken.token || rawToken.accessToken;
              } else {
                  token = rawToken;
              }
          }
          const userName = result.name || email.split('@')[0];

          saveSession({ name: userName, email: email }, token);
          showToast(`¡Bienvenido de nuevo, ${userName}!`);
          loginForm.reset();
          closeAuthModal();
        } else {
          showToast(result.message || 'Credenciales incorrectas', true);
        }
      } catch (error) {
        console.error('Error de red en el login:', error);
        showToast('No se pudo conectar con el servidor', true);
      }
    });
  }

});