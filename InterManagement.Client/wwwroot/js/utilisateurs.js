/* ==========================================================
   utilisateurs.js — UI de la page Utilisateurs (Admin)
   MIS À JOUR : gestion de la création de compte Identity
   avec affichage du mot de passe temporaire une seule fois.
========================================================== */

/* ============================================================
   TOGGLE DES CHAMPS SELON LE TYPE D'UTILISATEUR
============================================================ */
function toggleUserFields() {
  const type = document.querySelector('input[name="userType"]:checked').value;

  document
    .getElementById("stagiaireFields")
    .classList.toggle("active", type === "stagiaire");
  document
    .getElementById("mentorFields")
    .classList.toggle("active", type === "mentor");
  document
    .getElementById("adminFields")
    .classList.toggle("active", type === "admin");

  document
    .getElementById("userSpecialite")
    .closest(".form-group").style.display = type === "admin" ? "none" : "block";

  // "Terminé" n'a de sens que pour un Stagiaire (lié à TraineeStatus)
  const statutSelect = document.getElementById("userStatut");
  const termineOption = document.getElementById("userStatutTermineOption");
  if (termineOption) {
    termineOption.hidden = type !== "stagiaire";
    if (type !== "stagiaire" && statutSelect.value === "Completed") {
      statutSelect.value = "true";
      applyUserStatutSelection("true");
    }
  }

  updateFormAction(type);
}

/* ============================================================
   SYNCHRONISE LE SÉLECTEUR "Statut" AVEC IsActive ET Status
   Actif → IsActive=true,  Status=InProgress
   Inactif → IsActive=false, Status=Suspended
   Terminé → IsActive=true,  Status=Completed
============================================================ */
function applyUserStatutSelection(value) {
  const isActiveField = document.getElementById("userIsActiveField");
  const traineeStatusField = document.getElementById("userTraineeStatus");

  if (value === "Completed") {
    isActiveField.value = "true";
    traineeStatusField.value = "Completed";
  } else if (value === "false") {
    isActiveField.value = "false";
    traineeStatusField.value = "Suspended";
  } else {
    isActiveField.value = "true";
    traineeStatusField.value = "InProgress";
  }
}

function updateFormAction(type) {
  const form = document.getElementById("userForm");
  const userId = document.getElementById("userId").value;
  const isEdit = userId !== "";

  const actionMap = {
    stagiaire: isEdit
      ? `/Utilisateurs/EditStagiaire/${userId}`
      : "/Utilisateurs/CreateStagiaire",
    mentor: isEdit
      ? `/Utilisateurs/EditMentor/${userId}`
      : "/Utilisateurs/CreateMentor",
    admin: isEdit
      ? `/Utilisateurs/EditAdmin/${userId}`
      : "/Utilisateurs/CreateAdmin",
  };

  form.action = actionMap[type];
}

/* ============================================================
   MODAL UTILISATEUR — MODE AJOUT
============================================================ */
function openUserModal() {
  document.getElementById("userId").value = "";
  document.getElementById("userModalTitle").innerHTML =
    '<i class="fas fa-user"></i> Ajouter un utilisateur';

  document.querySelector('input[name="userType"][value="stagiaire"]').checked =
    true;
  toggleUserFields();

  [
    "userNom",
    "userPrenom",
    "userEmail",
    "userSpecialite",
    "userStructure",
    "userTheme",
    "userDateDebut",
    "userDateFin",
  ].forEach((id) => {
    const el = document.getElementById(id);
    if (el) el.value = "";
  });

  document.getElementById("userStatut").value = "true";
  applyUserStatutSelection("true");
  document.getElementById("userDepartement").value = "Engineering";
  document.getElementById("userRole").value = "Administrateur";

  openModal("userModal");
}

/* ============================================================
   MODAL UTILISATEUR — MODE ÉDITION
============================================================ */
function openEditUserModal(btn) {
  const row = btn.closest("tr");
  const type = row.dataset.userType;

  document.getElementById("userId").value = row.dataset.userId;
  document.querySelector(`input[name="userType"][value="${type}"]`).checked =
    true;
  toggleUserFields();

  document.getElementById("userModalTitle").innerHTML =
    `<i class="fas fa-user"></i> Modifier ${row.dataset.prenom} ${row.dataset.nom}`;

  document.getElementById("userNom").value = row.dataset.nom || "";
  document.getElementById("userPrenom").value = row.dataset.prenom || "";
  document.getElementById("userEmail").value = row.dataset.email || "";
  document.getElementById("userSpecialite").value =
    row.dataset.specialite || "";
  const rowIsActive = (row.dataset.statut || "Actif") === "Actif";
  const rowTraineeStatus = row.dataset.traineeStatus || "InProgress";

  // Reflète l'état réel du stagiaire (pas la correspondance par défaut)
  document.getElementById("userIsActiveField").value = String(rowIsActive);
  document.getElementById("userTraineeStatus").value = rowTraineeStatus;

  document.getElementById("userStatut").value = !rowIsActive
    ? "false"
    : rowTraineeStatus === "Completed"
      ? "Completed"
      : "true";

  if (type === "stagiaire") {
    document.getElementById("userStructure").value =
      row.dataset.structure || "";
    document.getElementById("userTheme").value = row.dataset.theme || "";
    document.getElementById("userDateDebut").value =
      row.dataset.dateDebut || "";
    document.getElementById("userDateFin").value = row.dataset.dateFin || "";
  } else if (type === "mentor") {
    document.getElementById("userDepartement").value =
      row.dataset.departement || "Engineering";
  }

  openModal("userModal");
}

/* ============================================================
   MODAL DÉTAIL STAGIAIRE
============================================================ */
// Reproduit le classement de _StatusBadge.cshtml côté client
// (le modal détail est rempli en JS, pas re-rendu par Razor).
function traineeStatusBadgeHtml(row) {
  const isActive = (row.dataset.statut || "Actif") === "Actif";
  const isCompleted = row.dataset.traineeStatus === "Completed";
  const label = !isActive ? "Inactif" : isCompleted ? "Terminé" : "Actif";
  const cssClass = !isActive
    ? "status-Inactif"
    : isCompleted
      ? "status-Completed"
      : "status-actif";

  return `<span class="status-badge ${cssClass}">${label}</span>`;
}

function openTraineeDetailsModal(btn) {
  const row = btn.closest("tr");
  if (!row) return;

  const firstName = row.dataset.prenom || "";
  const lastName = row.dataset.nom || "";
  const fullName = `${firstName} ${lastName}`.trim();
  const initials = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();

  const startDate = row.dataset.dateDebut || "";
  const endDate = row.dataset.dateFin || "";

  document.getElementById("detailTraineeAvatar").textContent = initials || "?";
  document.getElementById("detailTraineeFullName").textContent = fullName || "—";
  document.getElementById("detailTraineeEmail").textContent = row.dataset.email || "—";
  document.getElementById("detailTraineeSpecialty").textContent =
    row.dataset.specialite || "—";
  document.getElementById("detailTraineeUniversity").textContent =
    row.dataset.structure || "—";
  document.getElementById("detailTraineeTheme").textContent = row.dataset.theme || "—";
  document.getElementById("detailTraineeStartDate").textContent = startDate
    ? formatDate(startDate)
    : "—";
  document.getElementById("detailTraineeEndDate").textContent = endDate
    ? formatDate(endDate)
    : "—";
  document.getElementById("detailTraineeStatusBadge").innerHTML =
    traineeStatusBadgeHtml(row);

  openModal("traineeDetailsModal");
}

/* ============================================================
   MOT DE PASSE TEMPORAIRE
   Après création d'un utilisateur, le Server retourne
   un mot de passe temporaire → on l'affiche dans le modal
   #passwordModal une seule fois à l'Admin.
============================================================ */

/**
 * Affiche le modal avec le mot de passe temporaire.
 * Appelé par le Controller via TempData après création réussie.
 */
function showPasswordModal(password, email, role) {
  document.getElementById("tempPasswordDisplay").textContent = password;
  document.getElementById("tempPasswordEmail").textContent = email;
  document.getElementById("tempPasswordRole").textContent = role;
  openModal("passwordModal");
}

/**
 * Copie le mot de passe dans le presse-papier.
 */
function copyPassword() {
  const password = document.getElementById("tempPasswordDisplay").textContent;
  navigator.clipboard.writeText(password).then(() => {
    const btn = event.target.closest("button");
    btn.innerHTML = '<i class="fas fa-check"></i> Copié !';
    btn.style.background = "#28a745";
    setTimeout(() => {
      btn.innerHTML = '<i class="fas fa-copy"></i> Copier le mot de passe';
      btn.style.background = "#1e2242";
    }, 2000);
  });
}

/**
 * Ferme le modal mot de passe.
 */
function closePasswordModal() {
  closeModal("passwordModal");
  // Efface le mot de passe de l'écran pour la sécurité
  document.getElementById("tempPasswordDisplay").textContent = "—";
}

/* ============================================================
   INITIALISATION
============================================================ */
document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll('input[name="userType"]').forEach((radio) => {
    radio.addEventListener("change", toggleUserFields);
  });

  document
    .getElementById("openUserBtn")
    ?.addEventListener("click", openUserModal);

  document
    .getElementById("userStatut")
    ?.addEventListener("change", function () {
      applyUserStatutSelection(this.value);
    });

  // Si TempData contient un mot de passe temporaire → l'afficher
  // (valeur injectée par Razor dans un data-attribute)
  const passwordData = document.getElementById("passwordData");
  if (passwordData && passwordData.dataset.password) {
    showPasswordModal(
      passwordData.dataset.password,
      passwordData.dataset.email,
      passwordData.dataset.role,
    );
  }
});

/* ============================================================
   EXPOSITION GLOBALE
============================================================ */
window.openEditUserModal = openEditUserModal;
window.openTraineeDetailsModal = openTraineeDetailsModal;
window.toggleUserFields = toggleUserFields;
window.copyPassword = copyPassword;
window.closePasswordModal = closePasswordModal;
