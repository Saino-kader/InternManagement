/* ==========================================================
   phases.js — UI de la page Phases
   Gère : accordéon, modal "Ajouter une phase" (multi-sélection
   stagiaires + semaines dynamiques), modal "Ajouter une semaine",
   modal "Modifier une phase".
========================================================== */

let dynamicWeeksCount = 0;
let selectedStagiaires = [];

/* ============================================================
   MULTI-SÉLECTION STAGIAIRES
============================================================ */

function toggleMultiSelect() {
  document.getElementById('stagiairesDropdown').classList.toggle('active');
}

function initStagiairesCheckboxes() {
  document.querySelectorAll('.stagiaire-cb').forEach((cb) => {
    cb.addEventListener('change', function () {
      const name = this.dataset.name;
      if (this.checked) {
        if (!selectedStagiaires.includes(name)) selectedStagiaires.push(name);
      } else {
        const idx = selectedStagiaires.indexOf(name);
        if (idx > -1) selectedStagiaires.splice(idx, 1);
      }
      document.getElementById('selectedStagiairesText').innerText =
        selectedStagiaires.length
          ? selectedStagiaires.join(', ')
          : 'Aucun stagiaire sélectionné';
    });
  });
}

/* ============================================================
   SEMAINES DYNAMIQUES — MODAL AJOUTER UNE PHASE
============================================================ */

function createAddWeekButton() {
  const btn = document.createElement('button');
  btn.type = 'button';
  btn.className = 'btn-add-week';
  btn.id = 'addWeekBtnPhases';
  btn.innerHTML = '<i class="fas fa-plus"></i> Ajouter une semaine';
  btn.onclick = addNewWeek;
  return btn;
}

function buildWeekCardHTML(rowIndex, data = {}) {
  const weekNumber = data.weekNumber || rowIndex;
  return `
    <div class="week-card-modal" data-row-index="${rowIndex}">
      <button type="button" class="btn-remove-week" onclick="removeWeek(this)">
        <i class="fas fa-trash"></i>
      </button>
      <h4>Semaine</h4>
      <div class="form-group">
        <label>Numéro de semaine</label>
        <input type="number" min="1" class="week-number-input"
               value="${weekNumber}" required>
      </div>
      <div class="form-group">
        <label>Cours</label>
        <input type="text" class="week-course-input"
               placeholder="Nom du cours" value="${data.cours || ''}">
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Date début</label>
          <input type="date" class="week-start-input"
                 value="${data.debut || ''}" required>
        </div>
        <div class="form-group">
          <label>Date fin</label>
          <input type="date" class="week-end-input"
                 value="${data.fin || ''}" required>
        </div>
      </div>
    </div>`;
}

function addNewWeek() {
  dynamicWeeksCount++;
  const container = document.getElementById('phaseWeeksContainer');
  const existingBtn = document.getElementById('addWeekBtnPhases');
  const temp = document.createElement('div');
  temp.innerHTML = buildWeekCardHTML(dynamicWeeksCount);
  if (existingBtn) {
    container.insertBefore(temp.firstElementChild, existingBtn);
  } else {
    container.appendChild(temp.firstElementChild);
    container.appendChild(createAddWeekButton());
  }
}

function removeWeek(btn) {
  const card = btn.closest('.week-card-modal');
  if (card) card.remove();
  if (!document.querySelectorAll('#phaseWeeksContainer .week-card-modal').length) {
    addNewWeek();
  }
}

function prepareWeeksPayload() {
  const cards = Array.from(
    document.querySelectorAll('#phaseWeeksContainer .week-card-modal')
  );

  if (!cards.length)
    return { isValid: false, message: 'Veuillez ajouter au moins une semaine.' };

  const usedWeekNumbers = new Set();

  for (let i = 0; i < cards.length; i++) {
    const card = cards[i];
    const weekNumberInput = card.querySelector('.week-number-input');
    const courseInput     = card.querySelector('.week-course-input');
    const startInput      = card.querySelector('.week-start-input');
    const endInput        = card.querySelector('.week-end-input');

    const weekNumber = parseInt(weekNumberInput?.value || '0', 10);

    if (!Number.isInteger(weekNumber) || weekNumber <= 0)
      return { isValid: false, message: 'Chaque semaine doit avoir un numéro > 0.' };

    if (usedWeekNumbers.has(weekNumber))
      return { isValid: false, message: `Numéro de semaine ${weekNumber} en double.` };

    usedWeekNumbers.add(weekNumber);

    weekNumberInput.name = `Weeks[${i}].WeekNumber`;
    courseInput.name     = `Weeks[${i}].Course`;
    startInput.name      = `Weeks[${i}].StartDate`;
    endInput.name        = `Weeks[${i}].EndDate`;
  }

  return { isValid: true };
}

function initPhaseFormSubmit() {
  const form = document.getElementById('phaseForm');
  if (!form) return;
  form.addEventListener('submit', (e) => {
    const payload = prepareWeeksPayload();
    if (!payload.isValid) {
      e.preventDefault();
      alert(payload.message);
    }
  });
}

/* ============================================================
   MODAL AJOUTER UNE PHASE — OUVERTURE
============================================================ */

function openPhaseModal() {
  selectedStagiaires = [];
  dynamicWeeksCount  = 0;

  document.getElementById('selectedStagiairesText').innerText =
    'Aucun stagiaire sélectionné';
  document.querySelectorAll('.stagiaire-cb').forEach((cb) => (cb.checked = false));
  document.getElementById('phaseModalTitle').innerHTML =
    '<i class="fas fa-diagram-project"></i> Ajouter une phase';
  document.getElementById('phaseNumero').value = '';
  document.getElementById('phaseTitre').value  = '';
  document.getElementById('phaseDateDebut').value = '';
  document.getElementById('phaseDateFin').value   = '';
  document.getElementById('phaseMentor').value    = '';

  const weeksContainer = document.getElementById('phaseWeeksContainer');
  weeksContainer.innerHTML = '';
  weeksContainer.appendChild(createAddWeekButton());
  addNewWeek();

  openModal('phaseModal');
}

/* ============================================================
   MODAL AJOUTER UNE SEMAINE — OUVERTURE
   Appelé depuis le bouton "+ Semaine" de chaque bloc stagiaire.
   Injecte le PhaseId dans le champ caché du formulaire.
============================================================ */

function openAddWeekModal(phaseId) {
  // Réinitialise les champs
  document.getElementById('addWeekPhaseId').value = phaseId;
  document.getElementById('addWeekNumber').value  = '';
  document.getElementById('addWeekCourse').value  = '';
  document.getElementById('addWeekStart').value   = '';
  document.getElementById('addWeekEnd').value     = '';
  openModal('addWeekModal');
}

/* ============================================================
   MODAL MODIFIER UNE PHASE — OUVERTURE
   Appelé depuis le bouton crayon de chaque ligne de semaine.
   Lit les data-attributes de ce bouton pour pré-remplir.
============================================================ */

function normalizePhaseStatus(status) {
  const v = (status || '').trim().toLowerCase();
  if (v === 'inprogress' || v === 'en cours') return 'InProgress';
  if (v === 'validated' || v === 'validé')    return 'Validated';
  if (v === 'suspended' || v === 'suspendu')  return 'Suspended';
  return 'InProgress';
}

function openPhaseEditModal(btn) {
  if (!btn || btn.disabled) return;

  const phaseId  = btn.dataset.phaseId;
  const weekId   = btn.dataset.weekId;
  if (!phaseId || !weekId || weekId === '0') return;

  document.getElementById('editPhaseId').value       = phaseId;
  document.getElementById('editWeekId').value        = weekId;
  document.getElementById('editTraineeName').value   = btn.dataset.traineeName || '';
  document.getElementById('editMentorName').value    = btn.dataset.mentorName  || '';
  document.getElementById('editStatus').value        = normalizePhaseStatus(btn.dataset.phaseStatus);
  document.getElementById('editWeekStatus').value    = normalizePhaseStatus(btn.dataset.weekStatus);
  document.getElementById('editWeekNumber').textContent = btn.dataset.weekNumber || '-';
  document.getElementById('editCourse').value        = btn.dataset.course    || '';
  document.getElementById('editWeekStartDate').value = btn.dataset.startDate || '';
  document.getElementById('editWeekEndDate').value   = btn.dataset.endDate   || '';

  openModal('phaseEditModal');
}

/* ============================================================
   ONGLETS SEMAINES (présent pour compatibilité future)
============================================================ */

function switchWeekTab(btn) {
  const tabsContainer = btn.closest('.week-tabs');
  tabsContainer.querySelectorAll('.week-tab').forEach((t) => t.classList.remove('active'));
  btn.classList.add('active');

  const accordionBody = btn.closest('.phase-accordion-body');
  const targetId = btn.dataset.target;
  accordionBody?.querySelectorAll('.phase-week-panel')
    .forEach((p) => p.classList.remove('active'));
  if (targetId)
    accordionBody?.querySelector(`#${targetId}`)?.classList.add('active');
}

/* ============================================================
   INITIALISATION
============================================================ */
document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('openPhaseBtn')?.addEventListener('click', openPhaseModal);
  initStagiairesCheckboxes();
  initPhaseFormSubmit();
  initDeleteButtons();
});

/* ============================================================
   EXPOSITION GLOBALE
============================================================ */
window.toggleMultiSelect   = toggleMultiSelect;
window.removeWeek          = removeWeek;
window.addNewWeek          = addNewWeek;
window.switchWeekTab       = switchWeekTab;
window.openPhaseEditModal  = openPhaseEditModal;
window.openAddWeekModal    = openAddWeekModal;