/* ==========================================================
   mentor-space.js — UI de l'espace Mentor
   Gère : modal détail stagiaire, modal commentaire, pré-
   remplissage du formulaire feedback, header scroll, curseur,
   panneau compte.
   AUCUNE donnée mockée — currentMentor, stagiairesAssignes,
   phasesSemaines et feedbacksHistorique du fichier original
   ont été entièrement supprimés. Les tableaux sont désormais
   générés par Razor (_StagiairesAssignesTable.cshtml,
   _HistoriqueFeedbacksTable.cshtml) à partir des vraies
   données de l'API.
========================================================== */

/* ============================================================
   MODALS — OUVERTURE / FERMETURE
   (page autonome, pas de site.js partagé ici puisque cette
   page n'utilise pas le Layout Admin)
============================================================ */

function openModal(modalId) {
  document.getElementById(modalId).classList.add("active");
  document.body.style.overflow = "hidden";
}

function closeModal(modalId) {
  document.getElementById(modalId).classList.remove("active");
  document.body.style.overflow = "auto";
}

/* ============================================================
   VOIR LES DÉTAILS D'UN STAGIAIRE
   Lit les data-attributes de la ligne <tr> cliquée
   (générés par Razor) — plus de recherche dans un tableau JS.
============================================================ */

function viewStagiaireDetails(btn) {
  const row = btn.closest("tr");

  document.getElementById("viewStagiaireContent").innerHTML = `
    <div style="margin-bottom: 1.5rem;">
      <p><strong>Nom complet :</strong> ${row.dataset.stagiaireNom}</p>
      <p><strong>Phase actuelle :</strong> ${row.dataset.phase}</p>
      <p><strong>Semaine actuelle :</strong> Semaine ${row.dataset.semaine}</p>
      <p><strong>Cours :</strong> ${row.dataset.cours}</p>
      <p><strong>Date début :</strong> ${formatDate(row.dataset.dateDebut)}</p>
      <p><strong>Date fin :</strong> ${formatDate(row.dataset.dateFin)}</p>
      <p><strong>Statut :</strong> ${row.dataset.statut}</p>
    </div>`;

  openModal("viewStagiaireModal");
}

/* ============================================================
   VOIR LE COMMENTAIRE COMPLET D'UN FEEDBACK
============================================================ */

function viewComment(icon) {
  const row = icon.closest("tr");

  document.getElementById("viewCommentContent").innerHTML = `
    <div>
      <p><strong>Stagiaire :</strong> ${row.dataset.stagiaireNom}</p>
      <p><strong>Date :</strong> ${formatDate(row.dataset.date)}</p>
      <hr style="margin: 1rem 0;">
      <p><strong>Commentaire :</strong></p>
      <p style="background: var(--bg); padding: 1rem; border-radius: 16px; margin-top: 0.5rem;">
        ${row.dataset.commentaire}
      </p>
    </div>`;

  openModal("viewCommentModal");
}

/* ============================================================
   PRÉ-REMPLIR LE FORMULAIRE FEEDBACK
   Depuis le bouton "feedback" d'une ligne de stagiaire assigné.
============================================================ */

function prefillFeedbackForm(btn) {
  const row = btn.closest("tr");

  document.getElementById("feedbackStagiaire").value = row.dataset.traineeId;

  // Scroll vers le formulaire
  document
    .querySelector(".feedback-premium")
    ?.scrollIntoView({ behavior: "smooth" });
}

/* ============================================================
   FORMATAGE DE DATE
============================================================ */

function formatDate(dateStr) {
  if (!dateStr) return "";
  return new Date(dateStr).toLocaleDateString("fr-FR");
}

/* ============================================================
   SUPPRESSION — CONFIRMATION GÉNÉRIQUE
   Même mécanique que site.js (data-delete-url / data-delete-name)
   redéclarée ici car cette page n'inclut pas site.js.
============================================================ */

let deleteCallback = null;

function initDeleteButtons() {
  document
    .querySelectorAll(
      ".btn-delete[data-delete-url], .action-btn.delete[data-delete-url], .table-action-btn.delete[data-delete-url]",
    )
    .forEach((btn) => {
      btn.addEventListener("click", (e) => {
        e.preventDefault();
        const url = btn.dataset.deleteUrl;
        const name = btn.dataset.deleteName || "cet élément";

        document.getElementById("deleteModalMessage").textContent =
          `Êtes-vous sûr de vouloir supprimer ${name} ? Cette action est irréversible.`;

        deleteCallback = () => {
          const form = document.createElement("form");
          form.method = "POST";
          form.action = url;

          const token = document.querySelector(
            'input[name="__RequestVerificationToken"]',
          );

          if (token) {
            const hidden = document.createElement("input");
            hidden.type = "hidden";
            hidden.name = "__RequestVerificationToken";
            hidden.value = token.value;
            form.appendChild(hidden);
          }

          document.body.appendChild(form);
          form.submit();
        };

        openModal("deleteModal");
      });
    });

  document.getElementById("confirmDeleteBtn")?.addEventListener("click", () => {
    if (deleteCallback) {
      deleteCallback();
      deleteCallback = null;
    }
    closeModal("deleteModal");
  });
}

/* ============================================================
   HEADER — EFFET AU SCROLL
============================================================ */

function initHeaderScroll() {
  window.addEventListener("scroll", () => {
    const header = document.getElementById("header");
    if (window.scrollY > 50) {
      header.classList.add("header-scrolled");
    } else {
      header.classList.remove("header-scrolled");
    }
  });
}

/* ============================================================
   CURSEUR PERSONNALISÉ
============================================================ */

function initCursor() {
  const cursor = document.querySelector(".cursor");
  const follower = document.querySelector(".cursor-follower");
  if (!cursor || !follower) return;

  document.addEventListener("mousemove", (e) => {
    cursor.style.transform = `translate(${e.clientX - 4}px, ${e.clientY - 4}px)`;
    follower.style.transform = `translate(${e.clientX - 20}px, ${e.clientY - 20}px)`;
  });

  document
    .querySelectorAll(
      "a, button, .phase-orbit-btn, .week-card, .stat-mini-card",
    )
    .forEach((el) => {
      el.addEventListener("mouseenter", () => {
        follower.style.transform = "scale(1.5)";
        follower.style.opacity = "0.3";
      });
      el.addEventListener("mouseleave", () => {
        follower.style.transform = "scale(1)";
        follower.style.opacity = "0.5";
      });
    });
}

/* ============================================================
   PANNEAU COMPTE
============================================================ */

function initAccountPanel() {
  const accountTrigger = document.getElementById("accountTrigger");
  const accountPanel = document.getElementById("accountPanel");
  if (!accountTrigger || !accountPanel) return;

  accountTrigger.addEventListener("click", (e) => {
    e.stopPropagation();
    accountPanel.classList.toggle("active");
  });

  document.addEventListener("click", (e) => {
    if (
      !accountPanel.contains(e.target) &&
      !accountTrigger.contains(e.target)
    ) {
      accountPanel.classList.remove("active");
    }
  });
}

/* ============================================================
   MODAL DE CONNEXION (même comportement que les pages publiques)
============================================================ */

function initLoginModal() {
  const loginTrigger = document.getElementById("loginTrigger");
  const loginModal = document.getElementById("loginModal");
  const loginBackdrop = document.getElementById("loginBackdrop");
  const loginModalClose = document.getElementById("loginModalClose");
  const passwordToggle = document.getElementById("passwordToggle");
  const loginPassword = document.getElementById("loginPassword");

  if (!loginTrigger || !loginModal) return;

  loginTrigger.addEventListener("click", (e) => {
    e.preventDefault();
    e.stopPropagation();
    loginModal.classList.add("active");
    document.body.style.overflow = "hidden";
  });

  loginModalClose?.addEventListener("click", () => {
    loginModal.classList.remove("active");
    document.body.style.overflow = "auto";
  });

  loginBackdrop?.addEventListener("click", () => {
    loginModal.classList.remove("active");
    document.body.style.overflow = "auto";
  });

  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && loginModal.classList.contains("active")) {
      loginModal.classList.remove("active");
      document.body.style.overflow = "auto";
    }
  });

  if (passwordToggle && loginPassword) {
    passwordToggle.addEventListener("click", () => {
      const icon = passwordToggle.querySelector("i");
      if (loginPassword.type === "password") {
        loginPassword.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
      } else {
        loginPassword.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
      }
    });
  }

  loginModal.addEventListener("click", (e) => {
    e.stopPropagation();
  });
}

/* ============================================================
   LIENS STAGIAIRE / MENTOR — ouvrent le modal si la route
   directe n'est pas disponible (fallback des pages publiques)
============================================================ */

function initProtectedNavLinks() {
  const loginModal = document.getElementById("loginModal");
  if (!loginModal) return;

  ["navStagiaireLink", "navMentorLink"].forEach((id) => {
    document.getElementById(id)?.addEventListener("click", (e) => {
      e.preventDefault();
      loginModal.classList.add("active");
      document.body.style.overflow = "hidden";
    });
  });
}

/* ============================================================
   INITIALISATION
   Note : le formulaire feedback n'a plus de gestionnaire JS
   de soumission personnalisé — il poste nativement vers
   MentorSpaceController.SendFeedback (asp-controller/asp-action).
   La validation "required" HTML remplace l'ancienne vérification
   manuelle (if (!stagiaireId || !phase...) { alert(...) }).
============================================================ */
document.addEventListener("DOMContentLoaded", () => {
  initHeaderScroll();
  initCursor();
  initAccountPanel();
  initLoginModal();
  initProtectedNavLinks();
  initDeleteButtons();
});

/* ============================================================
   EXPOSITION GLOBALE
   Nécessaire pour les onclick="..." dans le HTML Razor
============================================================ */
window.openModal = openModal;
window.closeModal = closeModal;
window.viewStagiaireDetails = viewStagiaireDetails;
window.viewComment = viewComment;
window.prefillFeedbackForm = prefillFeedbackForm;
