/* ==========================================================
   site.js — Fonctions UI globales
   Chargé sur TOUTES les pages de l'espace Admin
   Gère : sidebar, navigation, modals, accordéon, notifications
   AUCUNE donnée métier ici — uniquement de l'UI/UX
========================================================== */

/* ============================================================
   VARIABLE D'ÉTAT GLOBALE
   (callback de confirmation de suppression, partagé entre pages)
============================================================ */
let deleteCallback = null;

/* ============================================================
   MODALS — OUVERTURE / FERMETURE
============================================================ */

/**
 * Ouvre un modal par son ID.
 * @param {string} modalId
 */
function openModal(modalId) {
  const modal = document.getElementById(modalId);
  if (modal) modal.classList.add("active");
}

/**
 * Ferme un modal par son ID.
 * @param {string} modalId
 */
function closeModal(modalId) {
  const modal = document.getElementById(modalId);
  if (modal) modal.classList.remove("active");
}

/* ============================================================
   FORMATAGE — fonction utilitaire pure, sans donnée métier
============================================================ */

/**
 * Formate une date ISO (yyyy-mm-dd) en format français (jj/mm/aaaa).
 * @param {string} date
 * @returns {string}
 */
function formatDate(date) {
  return new Date(date).toLocaleDateString("fr-FR");
}

/* ============================================================
   SIDEBAR — TOGGLE MOBILE
============================================================ */

function initSidebarToggle() {
  const toggleBtn = document.querySelector(".menu-toggle");
  const sidebar = document.getElementById("sidebar");
  if (!toggleBtn || !sidebar) return;

  toggleBtn.addEventListener("click", () => {
    sidebar.classList.toggle("hide");
  });
}

/* ============================================================
   NAVIGATION — CHANGEMENT DE PAGE
   Affiche la page correspondant à l'item de menu cliqué,
   cache les autres via la classe .hidden-page
============================================================ */

function initNavigation() {
  const pageIds = [
    "dashboardPage",
    "utilisateursPage",
    "phasesPage",
    "suiviPage",
  ];

  document.querySelectorAll(".side-menu > li[data-page]").forEach((item) => {
    item.addEventListener("click", () => {
      const page = item.dataset.page;

      document
        .querySelectorAll(".side-menu > li")
        .forEach((li) => li.classList.remove("active"));
      item.classList.add("active");

      pageIds.forEach((id) => {
        const el = document.getElementById(id);
        if (el) el.classList.add("hidden-page");
      });

      const targetMap = {
        dashboard: "dashboardPage",
        utilisateurs: "utilisateursPage",
        phases: "phasesPage",
        suivi: "suiviPage",
      };

      const targetId = targetMap[page];
      if (targetId) {
        document.getElementById(targetId)?.classList.remove("hidden-page");
      }
    });
  });
}

/* ============================================================
   ACCORDÉON DES PHASES
   Ouvre/ferme chaque section de phase au clic sur son en-tête
============================================================ */

function initPhaseAccordion() {
  document.querySelectorAll(".phase-accordion-header").forEach((header) => {
    header.addEventListener("click", () => {
      const body = header.nextElementSibling;
      const icon = header.querySelector(".phase-toggle-icon");

      body.classList.toggle("active");
      icon.classList.toggle("fa-chevron-down");
      icon.classList.toggle("fa-chevron-right");
    });
  });

  // Ouvre la première phase par défaut
  const firstHeader = document.querySelector(".phase-accordion-header");
  if (firstHeader) firstHeader.click();
}

/* ============================================================
   ONGLETS DE TABLEAUX (Utilisateurs, Suivi)
   Chaque groupe ".tabs-section" contient une nav ".tab-nav" et
   ses panneaux ".tab-panel" ; un clic sur un bouton affiche le
   panneau correspondant (data-tab-target) et cache les autres.
============================================================ */

function initDataTabs() {
  document.querySelectorAll(".tabs-section").forEach((section) => {
    const buttons = section.querySelectorAll(".tab-btn");
    const panels = section.querySelectorAll(".tab-panel");

    buttons.forEach((btn) => {
      btn.addEventListener("click", () => {
        buttons.forEach((b) => b.classList.remove("active"));
        panels.forEach((p) => p.classList.remove("active"));

        btn.classList.add("active");
        section
          .querySelector(`.tab-panel[data-tab-panel="${btn.dataset.tabTarget}"]`)
          ?.classList.add("active");
      });
    });
  });
}

/* ============================================================
   NOTIFICATIONS — DROPDOWN CLOCHE
============================================================ */

function initNotificationDropdown() {
  const bell = document.getElementById("notificationBell");
  const dropdown = document.getElementById("notificationDropdown");
  if (!bell || !dropdown) return;

  bell.addEventListener("click", (e) => {
    e.stopPropagation();
    dropdown.classList.toggle("active");
  });

  document.addEventListener("click", () => {
    dropdown.classList.remove("active");
  });
}

/* ============================================================
   NOTIFICATIONS — FEEDBACKS ADMIN
   Charge tous les feedbacks visibles dans la cloche admin.
============================================================ */

async function loadAdminFeedbackNotifications() {
  const notificationList = document.getElementById("notificationList");
  const notificationBadge = document.getElementById("notificationBadge");
  if (!notificationList || !notificationBadge) return;

  try {
    const response = await fetch("/api-client/feedback", {
      headers: { Accept: "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const feedbacks = await response.json();
    const items = Array.isArray(feedbacks)
      ? feedbacks
          .slice()
          .sort((left, right) => new Date(right.sentAt) - new Date(left.sentAt))
      : [];

    notificationBadge.textContent = String(items.length);

    if (items.length === 0) {
      notificationList.innerHTML = `
        <p style="padding: 1rem; color: var(--text-soft); font-size: 0.85rem;">
          Aucun feedback disponible
        </p>`;
      return;
    }

    notificationList.innerHTML = items
      .map((feedback) => {
        const message = (feedback.message || "").trim();
        const source = feedback.senderType || "Feedback";
        const sentAt = feedback.sentAt
          ? new Date(feedback.sentAt).toLocaleString("fr-FR")
          : "";
        const preview =
          message.length > 120 ? `${message.slice(0, 120)}...` : message;

        return `
          <a href="/Feedback/Details/${feedback.id}" class="notification-item" style="display:block; text-decoration:none; color:inherit;">
            <div class="notification-item-header">
              <span class="notification-stagiaire">${feedback.traineeName || "Stagiaire inconnu"}</span>
              <span class="notification-date">${sentAt}</span>
            </div>
            <div class="notification-phase">${source}</div>
            <div class="notification-cours">${preview}</div>
          </a>`;
      })
      .join("");
  } catch (error) {
    console.error("Impossible de charger les notifications feedback:", error);
    notificationBadge.textContent = "0";
    notificationList.innerHTML = `
      <p style="padding: 1rem; color: var(--text-soft); font-size: 0.85rem;">
        Impossible de charger les feedbacks
      </p>`;
  }
}

/* ============================================================
   SUPPRESSION — MODAL DE CONFIRMATION GÉNÉRIQUE
   Utilisé par toutes les pages (stagiaire, mentor, admin,
   phase, suivi, message) pour confirmer une suppression
   avant d'envoyer la requête DELETE au backend.
============================================================ */


function openDeleteConfirm(message, callback) {
  document.getElementById("deleteModalMessage").innerHTML = message;
  deleteCallback = callback;
  openModal("deleteModal");
}

function initDeleteConfirmButton() {
  const confirmBtn = document.getElementById("confirmDeleteBtn");
  if (!confirmBtn) return;

  confirmBtn.addEventListener("click", () => {
    if (deleteCallback) {
      deleteCallback();
      deleteCallback = null;
    }
    closeModal("deleteModal");
  });
}

/* ============================================================
   SUPPRESSION — BOUTONS data-delete-url
   Parcourt tous les .action-btn.delete et attache
   un écouteur qui ouvre le modal de confirmation puis
   soumet un formulaire POST à l'URL spécifiée.
============================================================ */

function initDeleteButtons() {
  document
    .querySelectorAll(
      ".action-btn.delete[data-delete-url], .action-btn.delete[data-delete-form-id], .table-action-btn.delete[data-delete-url], .table-action-btn.delete[data-delete-form-id]",
    )
    .forEach((btn) => {
      btn.addEventListener("click", (e) => {
        e.preventDefault();
        const url = btn.dataset.deleteUrl;
        const formId = btn.dataset.deleteFormId;
        const name = btn.dataset.deleteName || "cet élément";

        openDeleteConfirm(
          `Voulez-vous vraiment supprimer <strong>${name}</strong> ?<br>Cette action est irréversible.`,
          () => {
            if (formId) {
              const existingForm = document.getElementById(formId);
              if (existingForm) {
                existingForm.submit();
                return;
              }
            }

            if (!url) {
              return;
            }

            // Crée un formulaire POST dynamique (avec antiforgery)
            const form = document.createElement("form");
            form.method = "POST";
            form.action = url;

            // Ajoute le token anti-forgery s'il existe
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
          },
        );
      });
    });
}


/* ============================================================
   RECHERCHE GLOBALE — barre de recherche dans la Navbar
   Filtre les lignes de tableau visibles sur la page active
============================================================ */

function initSearch() {
    const searchInput = document.querySelector('nav .search-wrapper input');
    if (!searchInput) return;

    searchInput.addEventListener('input', function () {
        const term = this.value.toLowerCase().trim();
        filterTables(term);
    });
}

function filterTables(term) {
    // Cible toutes les lignes de tous les tableaux de la page
    const rows = document.querySelectorAll('.data-table tbody tr');

    rows.forEach(row => {
        // Si la ligne est une ligne "vide" (colspan) → toujours visible
        if (row.querySelector('td[colspan]')) {
            row.style.display = '';
            return;
        }

        // Cherche le terme dans tout le texte de la ligne
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(term) ? '' : 'none';
    });

    // Affiche un message si aucun résultat
    showNoResultsMessage(term);
}

function showNoResultsMessage(term) {
    document.querySelectorAll('.data-table').forEach(table => {
        const tbody   = table.querySelector('tbody');
        const visible = Array.from(tbody.querySelectorAll('tr'))
            .filter(r => r.style.display !== 'none' && !r.querySelector('td[colspan]'));

        // Retire l'ancien message si présent
        const old = tbody.querySelector('.no-search-results');
        if (old) old.remove();

        // Ajoute un message si aucun résultat et terme non vide
        if (visible.length === 0 && term !== '') {
            const cols = table.querySelector('thead tr')
                ?.querySelectorAll('th').length || 5;
            const msg = document.createElement('tr');
            msg.className = 'no-search-results';
            msg.innerHTML = `
                <td colspan="${cols}"
                    style="text-align:center; padding:2rem;
                           color:var(--text-light);">
                    <i class="fas fa-search"
                       style="margin-right:6px;"></i>
                    Aucun résultat pour "<strong>${term}</strong>"
                </td>`;
            tbody.appendChild(msg);
        }
    });
}


/* ============================================================
   POINT D'ENTRÉE GLOBAL
============================================================ */
document.addEventListener("DOMContentLoaded", () => {
  initSidebarToggle();
  initNavigation();
  initPhaseAccordion();
  initDataTabs();
  initNotificationDropdown();
  initDeleteConfirmButton();
  initDeleteButtons(); 
  loadAdminFeedbackNotifications();
  initSearch(); 
});

/* ============================================================
   EXPOSITION GLOBALE
   Nécessaire pour les onclick="..." dans le HTML Razor
============================================================ */
window.openModal = openModal;
window.closeModal = closeModal;
window.formatDate = formatDate;
window.openDeleteConfirm = openDeleteConfirm;
