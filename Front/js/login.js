const API_BASE_URL = 'https://localhost:7281/api/Auth';

// --- HELPER: Decodificar Token JWT de C# ---
const parseJwt = (token) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
};

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

    window.dispatchEvent(new Event('authStateChanged'));
  };

  const clearSession = () => {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    updateUI();

    window.dispatchEvent(new Event('authStateChanged'));
  };

  const getSession = () => {
    const session = localStorage.getItem('currentUser');
    return session ? JSON.parse(session) : null;
  };

  // --- CONSULTAR BILLETERA AL BACKEND ---
window.fetchWalletBalance = async () => {
    const token = localStorage.getItem('token');
    const currentUser = getSession();
    
    if (!token || !currentUser || !currentUser.id) return;

    try {
        const response = await fetch(`https://localhost:7281/api/Wallet/user/${currentUser.id}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const apiResponse = await response.json();
            const wallet = apiResponse.data || apiResponse;

            // 1. Obtener el saldo disponible
            const balance = wallet.availableBalance !== undefined ? wallet.availableBalance : 
                           (wallet.AvailableBalance !== undefined ? wallet.AvailableBalance : (wallet.balance || 0));

            // 2. Obtener el saldo retenido (mapeando correctamente a 'balanceHeld' / 'BalanceHeld')
            const retained = wallet.balanceHeld !== undefined ? wallet.balanceHeld : 
                            (wallet.BalanceHeld !== undefined ? wallet.BalanceHeld : 
                            (wallet.retainedBalance || wallet.RetainedBalance || wallet.retained || 0));

            // Guardar en el objeto de sesión
            currentUser.wallet = balance;
            currentUser.retained = retained;
            localStorage.setItem('currentUser', JSON.stringify(currentUser));
            
            // Refrescar la interfaz para que dibuje los nuevos valores
            updateUI();
        }
    } catch (error) {
        console.error('Error al obtener la billetera:', error);
    }
};

  // --- ACTUALIZAR INTERFAZ (UI) ---
 const updateUI = () => {
    const user = getSession();
    const openCreateModalBtn = document.getElementById('openCreateModalBtn');
    const walletDisplay = document.getElementById('walletDisplay');
    const userWalletBalance = document.getElementById('userWalletBalance');
    const retainedElement = document.getElementById('userWalletRetained');

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

      // Mostrar billetera y sus saldos
      if (walletDisplay) {
        walletDisplay.classList.remove('hidden');
        walletDisplay.style.display = 'inline-flex';
      }

      // 1. Mostrar saldo disponible
      if (userWalletBalance) {
        const saldo = user.wallet !== undefined ? user.wallet : 0;
        userWalletBalance.textContent = `$ ${Number(saldo).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
      }

      // 2. Mostrar saldo retenido (¡Ahora está dentro del usuario logueado!)
      if (retainedElement) {
        const retainedAmount = user.retained || 0;
        retainedElement.textContent = `$ ${Number(retainedAmount).toLocaleString('es-AR', { 
          minimumFractionDigits: 2, 
          maximumFractionDigits: 2 
        })}`;
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

      // Ocultar billetera si no hay sesión
      if (walletDisplay) {
        walletDisplay.classList.add('hidden');
        walletDisplay.style.display = 'none';
      }
    }
  };

  // Inicializar UI al cargar
  updateUI();
  const savedUser = getSession();
  if (savedUser && savedUser.id) {
      fetchWalletBalance();
  }

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

            if (window.location.pathname.includes('dashboard.html')) {
                window.location.replace('index.html');
            } else {
                showToast('Sesión cerrada correctamente');
            }
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
        const response = await fetch(`${API_BASE_URL}/register`, {
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
        const response = await fetch(`${API_BASE_URL}/login`, {
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

          // Extraer ID de usuario y sincronizar billetera
          const tokenData = parseJwt(token);
          const userId = result.id || result.userId || tokenData?.sub || tokenData?.nameid || tokenData?.id ||tokenData?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
          const userName = result.name || email.split('@')[0];

          saveSession({ id: Number(userId), name: userName, email: email, wallet: 0 }, token);
          
          await fetchWalletBalance();

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