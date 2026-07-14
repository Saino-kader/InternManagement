/* ==========================================================
   site-public.js — UI de la page d'accueil publique (Home)
   Gère : effet de scroll du header, curseur personnalisé,
   panneau compte, modal de connexion.
   AUCUNE logique d'authentification réelle ici — le formulaire
   poste vers /Account/Login (ASP.NET Core gère la vraie
   vérification d'email/mot de passe côté backend).
========================================================== */


/* ============================================================
   HEADER — EFFET AU SCROLL
============================================================ */

function initHeaderScroll() {
  window.addEventListener('scroll', () => {
    const header = document.getElementById('header');
    if (window.scrollY > 50) {
      header.classList.add('header-scrolled');
    } else {
      header.classList.remove('header-scrolled');
    }
  });
}


/* ============================================================
   PANNEAU COMPTE (dropdown header)
============================================================ */

function initAccountPanel() {
  const accountTrigger = document.getElementById('accountTrigger');
  const accountPanel = document.getElementById('accountPanel');
  if (!accountTrigger || !accountPanel) return;

  accountTrigger.addEventListener('click', (e) => {
    e.stopPropagation();
    accountPanel.classList.toggle('active');
  });

  document.addEventListener('click', (e) => {
    if (!accountPanel.contains(e.target) && !accountTrigger.contains(e.target)) {
      accountPanel.classList.remove('active');
    }
  });
}


/* ============================================================
   MODAL DE CONNEXION
============================================================ */

function initLoginModal() {
  const loginTrigger = document.getElementById('loginTrigger');
  const loginModal = document.getElementById('loginModal');
  const loginBackdrop = document.getElementById('loginBackdrop');
  const loginModalClose = document.getElementById('loginModalClose');
  const passwordToggle = document.getElementById('passwordToggle');
  const loginPassword = document.getElementById('loginPassword');

  if (!loginTrigger || !loginModal) return;

  loginTrigger.addEventListener('click', (e) => {
    e.preventDefault();
    e.stopPropagation();
    loginModal.classList.add('active');
    document.body.style.overflow = 'hidden';
  });

  // Autres déclencheurs du même modal (ex. bouton du hero)
  document.querySelectorAll('.login-trigger-hero').forEach((trigger) => {
    trigger.addEventListener('click', (e) => {
      e.preventDefault();
      loginModal.classList.add('active');
      document.body.style.overflow = 'hidden';
    });
  });

  loginModalClose?.addEventListener('click', () => {
    loginModal.classList.remove('active');
    document.body.style.overflow = 'auto';
  });

  loginBackdrop?.addEventListener('click', () => {
    loginModal.classList.remove('active');
    document.body.style.overflow = 'auto';
  });

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && loginModal.classList.contains('active')) {
      loginModal.classList.remove('active');
      document.body.style.overflow = 'auto';
    }
  });

  // Toggle affichage/masquage du mot de passe (UI uniquement,
  // ne stocke ni ne valide jamais le mot de passe côté JS)
  if (passwordToggle && loginPassword) {
    passwordToggle.addEventListener('click', () => {
      const icon = passwordToggle.querySelector('i');
      if (loginPassword.type === 'password') {
        loginPassword.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
      } else {
        loginPassword.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
      }
    });
  }

  loginModal.addEventListener('click', (e) => {
    e.stopPropagation();
  });
}


/* ============================================================
   LIENS STAGIAIRE / MENTOR — Ouvrent le modal de connexion
   (ces espaces sont protégés, pas d'accès direct sans login)
============================================================ */

function initProtectedNavLinks() {
  const loginModal = document.getElementById('loginModal');
  if (!loginModal) return;

  ['navStagiaireLink', 'navMentorLink'].forEach(id => {
    document.getElementById(id)?.addEventListener('click', (e) => {
      e.preventDefault();
      loginModal.classList.add('active');
      document.body.style.overflow = 'hidden';
    });
  });
}


/* ============================================================
   INITIALISATION
============================================================ */
document.addEventListener('DOMContentLoaded', () => {
  initHeaderScroll();
  initAccountPanel();
  initLoginModal();
  initProtectedNavLinks();
});
