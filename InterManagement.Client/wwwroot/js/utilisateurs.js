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

  updateFormAction(type);
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
  document.getElementById("userTraineeStatus").value = "InProgress";
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
  document.getElementById("userStatut").value =
    (row.dataset.statut || "Actif") === "Actif" ? "true" : "false";
  document.getElementById("userTraineeStatus").value =
    row.dataset.traineeStatus || "InProgress";

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
function openTraineeDetailsModal(btn) {
  const row = btn.closest("tr");
  if (!row) return;

  const fullName = `${row.dataset.prenom || ""} ${row.dataset.nom || ""}`.trim();
  const isActiveLabel = row.dataset.statut || "Actif";
  const traineeStatus = row.dataset.traineeStatus || "InProgress";
  const traineeStatusLabel =
    traineeStatus === "InProgress"
      ? "En cours"
      : traineeStatus === "Validated" || traineeStatus === "Completed"
        ? "Validé"
        : traineeStatus === "Suspended"
          ? "Suspendu"
          : traineeStatus;

  const startDate = row.dataset.dateDebut || "";
  const endDate = row.dataset.dateFin || "";

  document.getElementById("detailTraineeFullName").value = fullName;
  document.getElementById("detailTraineeEmail").value = row.dataset.email || "";
  document.getElementById("detailTraineeSpecialty").value = row.dataset.specialite || "";
  document.getElementById("detailTraineeUniversity").value = row.dataset.structure || "";
  document.getElementById("detailTraineeTheme").value = row.dataset.theme || "";
  document.getElementById("detailTraineeStartDate").value = startDate
    ? formatDate(startDate)
    : "";
  document.getElementById("detailTraineeEndDate").value = endDate
    ? formatDate(endDate)
    : "";
  document.getElementById("detailTraineeStatus").value =
    `${isActiveLabel} (${traineeStatusLabel})`;

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
