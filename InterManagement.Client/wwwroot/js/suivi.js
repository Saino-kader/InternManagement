/* ==========================================================
   suivi.js — UI de la page Suivi des stagiaires
   Gère : modal ajout/modification suivi, modal commentaire,
   aperçu du fichier Excel importé (lecture seule côté JS,
   l'enregistrement réel se fait par le backend).
   AUCUNE donnée mockée, AUCUNE validation métier ici —
   le backend ASP.NET Core valide réellement les données.
========================================================== */

/* ============================================================
   MODAL SUIVI — AJOUT / MODIFICATION
============================================================ */

/**
 * Ouvre le modal "Ajouter un suivi" en mode Ajout.
 * Les <select> Stagiaire/Mentor sont déjà remplis par Razor.
 */
function openAddSuiviModal() {
  document.getElementById("suiviModalTitle").innerHTML =
    '<i class="fas fa-chart-line"></i> Ajouter un suivi';

  document.getElementById("suiviDate").value = new Date()
    .toISOString()
    .split("T")[0];
  document.getElementById("suiviSemaine").value = "";
  document.getElementById("suiviCours").value = "";
  document.getElementById("suiviCommentaire").value = "";

  // Normalise et applique le statut par défaut
  const statusSelect = document.getElementById("suiviStatut");
  statusSelect.value = "InProgress";
  applyStatusSelectColor(statusSelect);

  document.getElementById("suiviSaveBtn").innerHTML = "Enregistrer";

  // Réinitialise l'action du formulaire pour Create
  const form = document.getElementById("suiviForm");
  form.action = "/Suivi/Create";
  const idInput = document.getElementById("suiviIdField");
  if (idInput) idInput.remove();

  openModal("suiviModal");
}

/**
 * Ouvre le modal "Modifier le suivi" en pré-remplissant
 * les champs depuis les data-attributes de la ligne cliquée.
 * @param {HTMLElement} btn - le bouton crayon cliqué
 */
function openEditSuiviModal(btn) {
  const row = btn.closest("tr");

  document.getElementById("suiviModalTitle").innerHTML =
    '<i class="fas fa-chart-line"></i> Modifier le suivi';

  document.getElementById("suiviStagiaire").value = row.dataset.traineeId || "";
  document.getElementById("suiviMentor").value = row.dataset.mentorId || "";
  document.getElementById("suiviDate").value = row.dataset.date || "";
  document.getElementById("suiviSemaine").value = row.dataset.weekId || "";
  document.getElementById("suiviCours").value = row.dataset.cours || "";
  document.getElementById("suiviAppreciation").value =
    row.dataset.appreciation || "";
  document.getElementById("suiviCommentaire").value =
    row.dataset.commentaire || "";

  // Normalise le statut et l'applique au select
  const normalizedStatus = normalizeFollowUpStatus(
    row.dataset.statut || "InProgress",
  );
  document.getElementById("suiviStatut").value = normalizedStatus;
  applyStatusSelectColor(document.getElementById("suiviStatut"));

  document.getElementById("suiviSaveBtn").innerHTML = "Enregistrer";

  // Change l'action du formulaire pour pointer vers Edit
  const form = document.getElementById("suiviForm");
  const suiviId = row.dataset.suiviId;
  form.action = "/Suivi/Edit/" + suiviId;
  // Ajoute un champ caché pour l'ID si besoin
  let idInput = document.getElementById("suiviIdField");
  if (!idInput) {
    idInput = document.createElement("input");
    idInput.type = "hidden";
    idInput.id = "suiviIdField";
    idInput.name = "id";
    form.prepend(idInput);
  }
  idInput.value = suiviId;

  openModal("suiviModal");
}

/* ============================================================
   MODAL COMMENTAIRE — AFFICHAGE LECTURE SEULE
   Affiche le commentaire complet d'une ligne de suivi
   à partir des data-attributes (pas de données mockées).
============================================================ */

/**
 * Affiche le commentaire complet du suivi dans un modal.
 * @param {HTMLElement} btn - le bouton "œil" cliqué
 */
function viewCommentaire(btn) {
  const row = btn.closest("tr");

  document.getElementById("viewCommentaireContent").innerHTML = `
    <div class="message-content">
      <p><strong>Stagiaire :</strong> ${row.dataset.stagiaireNom}</p>
      <p><strong>Mentor :</strong> ${row.dataset.mentorNom}</p>
      <p><strong>Semaine :</strong> ${row.dataset.weekNumber}</p>
      <p><strong>Cours :</strong> ${row.dataset.cours}</p>
      <hr>
      <p><strong>Commentaire :</strong></p>
      <p>${row.dataset.commentaire || "Aucun commentaire"}</p>
    </div>`;

  openModal("viewCommentaireModal");
}

/* ============================================================
   IMPORT EXCEL — APERÇU CÔTÉ NAVIGATEUR
   Lit le fichier choisi et affiche un aperçu AVANT envoi.
   Aucune validation métier ici : le backend revalide tout
   et retourne la liste réelle des erreurs après import.
============================================================ */

function openImportModal() {
  document.getElementById("importFile").value = "";
  document.getElementById("importPreview").innerHTML = "";
  document.getElementById("confirmImportBtn").style.display = "none";
  openModal("importExportModal");
}

/**
 * Télécharge un modèle Excel vide pour faciliter l'import.
 */
function downloadTemplate() {
  const template = [
    {
      Stagiaire: "Moussa Diallo",
      Mentor: "Sophie Martin",
      Date: "2025-01-12",
      Semaine: "Semaine 1",
      Cours: "Introduction",
      Appreciation: "Très bien",
      Commentaire: "Bon travail",
      Statut: "Validé",
    },
  ];

  const ws = XLSX.utils.json_to_sheet(template);
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Template_Suivi");
  XLSX.writeFile(wb, "template_suivi_stagiaires.xlsx");
}

/**
 * Lit le fichier Excel choisi et affiche un simple aperçu
 * (nombre de lignes + 5 premières lignes). Aucune validation
 * métier : c'est uniquement pour que l'Admin visualise
 * ce qui sera envoyé au backend.
 */
function initExcelPreview() {
  const fileInput = document.getElementById("importFile");
  if (!fileInput) return;

  fileInput.addEventListener("change", function (e) {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.onload = function (evt) {
      const data = new Uint8Array(evt.target.result);
      const workbook = XLSX.read(data, { type: "array" });
      const sheet = workbook.Sheets[workbook.SheetNames[0]];
      const rows = XLSX.utils.sheet_to_json(sheet);

      if (rows.length === 0) {
        document.getElementById("importPreview").innerHTML =
          '<div class="preview-error">Aucune donnée trouvée dans le fichier.</div>';
        document.getElementById("confirmImportBtn").style.display = "none";
        return;
      }

      let previewHtml = `<div class="preview-valid">${rows.length} ligne(s) lue(s)</div>`;
      previewHtml += `<div style="margin-top:16px;"><strong>Aperçu (5 premières lignes) :</strong></div>`;
      previewHtml += '<table class="preview-table"><thead><tr>';

      const headers = Object.keys(rows[0]);
      headers.forEach((h) => (previewHtml += `<th>${h}</th>`));
      previewHtml += "</tr></thead><tbody>";

      rows.slice(0, 5).forEach((row) => {
        previewHtml += "<tr>";
        headers.forEach((h) => (previewHtml += `<td>${row[h] ?? ""}</td>`));
        previewHtml += "</tr>";
      });

      if (rows.length > 5) {
        previewHtml += `<tr><td colspan="${headers.length}">... et ${rows.length - 5} autre(s) ligne(s)</td></tr>`;
      }

      previewHtml += "</tbody></table>";

      document.getElementById("importPreview").innerHTML = previewHtml;
      document.getElementById("confirmImportBtn").style.display = "inline-flex";
    };

    reader.readAsArrayBuffer(file);
  });
}

/* ============================================================
   STATUS NORMALIZATION
   Convertit les valeurs d'entrée de statut vers les valeurs
   de l'enum backend du suivi (InProgress, Validated, Suspended)
============================================================ */

/**
 * Convertit une valeur de statut vers les valeurs de l'enum backend.
 * Accepte les variantes en français et anglais.
 * @param {string} status
 * @returns {string} - Une des valeurs : InProgress, Validated, Suspended
 */
function normalizeFollowUpStatus(status) {
  const value = (status || "").trim().toLowerCase();

  if (
    value === "inprogress" ||
    value === "encours" ||
    value === "encour" ||
    value === "en cours"
  ) {
    return "InProgress";
  }

  if (value === "validated" || value === "validé" || value === "valide") {
    return "Validated";
  }

  if (value === "suspended" || value === "suspendu") {
    return "Suspended";
  }

  return "InProgress"; // valeur par défaut
}

/**
 * Applique une classe CSS de couleur au select du statut en fonction
 * de la valeur sélectionnée (pour une rétroaction visuelle).
 * @param {HTMLSelectElement} selectElement
 */
function applyStatusSelectColor(selectElement) {
  if (!selectElement) return;

  const value = selectElement.value;
  selectElement.classList.remove(
    "status-select-inprogress",
    "status-select-validated",
    "status-select-suspended",
  );

  if (value === "InProgress") {
    selectElement.classList.add("status-select-inprogress");
  } else if (value === "Validated") {
    selectElement.classList.add("status-select-validated");
  } else if (value === "Suspended") {
    selectElement.classList.add("status-select-suspended");
  }
}

/**
 * Initialise les event listeners pour que le select du statut applique
 * sa couleur quand la sélection change.
 */
function initFollowUpStatusColorListener() {
  const statusSelect = document.getElementById("suiviStatut");
  if (!statusSelect) return;

  // Applique la couleur initiale
  applyStatusSelectColor(statusSelect);

  // Applique la couleur à chaque changement
  statusSelect.addEventListener("change", function () {
    applyStatusSelectColor(this);
  });
}

/**
 * Le bouton "Exporter" déclenche un téléchargement de fichier : la page
 * ne navigue jamais, donc le loader global de _Layout.cshtml ne s'affiche
 * pas pour ce lien (voir son exclusion pour la classe "export"). Sans
 * retour visuel, le clic peut sembler ne rien faire pendant les quelques
 * secondes de génération du fichier. On affiche donc une icône de
 * chargement directement sur le bouton, remise à l'état normal après un
 * court délai.
 */
function initExportFeedback() {
  document.querySelectorAll(".action-btn.export").forEach((btn) => {
    btn.addEventListener("click", () => {
      const icon = btn.querySelector("i");
      if (!icon) return;

      const originalClass = icon.className;
      icon.className = "fas fa-circle-notch fa-spin";

      setTimeout(() => {
        icon.className = originalClass;
      }, 3000);
    });
  });
}

/* ============================================================
   INITIALISATION — ÉVÉNEMENTS PROPRES À CETTE PAGE
============================================================ */
document.addEventListener("DOMContentLoaded", () => {
  document
    .getElementById("openAddSuiviBtn")
    ?.addEventListener("click", openAddSuiviModal);
  document
    .getElementById("openImportBtn")
    ?.addEventListener("click", openImportModal);
  document
    .getElementById("downloadTemplateBtn")
    ?.addEventListener("click", downloadTemplate);
  initExcelPreview();
  initFollowUpStatusColorListener();
  initExportFeedback();
});

/* ============================================================
   EXPOSITION GLOBALE
   Nécessaire pour les onclick="..." dans le HTML Razor
============================================================ */
window.openEditSuiviModal = openEditSuiviModal;
window.viewCommentaire = viewCommentaire;
