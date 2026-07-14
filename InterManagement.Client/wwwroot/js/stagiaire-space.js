/* ==========================================================
   stagiaire-space.js — UI de l'espace Stagiaire
   Gère : onglets phases, curseur, panneau compte, modal
   connexion, header scroll.
   AUCUNE donnée mockée — les cartes de semaines sont déjà
   générées par Razor (_WeekContent.cshtml). Le formulaire
   feedback poste réellement au backend (plus d'alert() fake).
========================================================== */


/* ============================================================
   ONGLETS DE NAVIGATION DES PHASES
============================================================ */

function initPhaseTabs() {
  const btns = document.querySelectorAll('.phase-orbit-btn');

  btns.forEach((btn) => {
    btn.addEventListener('click', () => {
      const phaseNum = btn.dataset.phase;

      btns.forEach((b) => b.classList.remove('active'));
      btn.classList.add('active');

      document.querySelectorAll('.phase-dimension').forEach((div) => {
        div.classList.remove('active');
      });

      document.getElementById(`phase${phaseNum}`)?.classList.add('active');
    });
  });
}


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
   PANNEAU COMPTE
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
   (présent ici uniquement si le stagiaire arrive sur cette
   page alors que sa session a expiré — cas géré plus tard
   par le module d'authentification)
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
   LIENS STAGIAIRE / MENTOR — ouvrent le modal si la route
   directe n'est pas disponible (fallback des pages publiques)
============================================================ */

function initProtectedNavLinks() {
  const loginModal = document.getElementById('loginModal');
  if (!loginModal) return;

  ['navStagiaireLink', 'navMentorLink'].forEach((id) => {
    document.getElementById(id)?.addEventListener('click', (e) => {
      e.preventDefault();
      loginModal.classList.add('active');
      document.body.style.overflow = 'hidden';
    });
  });
}


/* ============================================================
   INITIALISATION
   Note : le formulaire feedback n'a plus de gestionnaire JS
   personnalisé — il poste nativement vers le Controller MVC
   (asp-controller/asp-action), comme un vrai formulaire HTML.
   Le bouton "required" sur le textarea remplace la validation
   JS manuelle qui existait avant (if (!message.trim())...).
============================================================ */
document.addEventListener('DOMContentLoaded', () => {
  initPhaseTabs();
  initHeaderScroll();
  initAccountPanel();
  initLoginModal();
  initProtectedNavLinks();
});
