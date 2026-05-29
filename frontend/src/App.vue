<template>
  <router-view />
</template>

<script setup>
</script>

<style>
/* Global layouts details */
.app-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1.5rem;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

/* Common Components */
.card-title {
  font-size: 1.15rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1.25rem;
  border-bottom: 1px solid var(--border-glass);
  padding-bottom: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

/* SECTION 1: DASHBOARD GRID */
.dashboard-grid {
  display: grid;
  grid-template-columns: 320px 1fr;
  gap: 2rem;
}

.stats-sidebar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 1.25rem;
  background: var(--bg-card);
}

.stat-icon {
  font-size: 2rem;
  background: var(--surface-2);
  padding: 0.5rem;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-glass);
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-label {
  font-size: 0.8rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-primary);
}

.manage-categories-btn {
  width: 100%;
  padding: 0.85rem;
  font-size: 0.95rem;
  margin-top: 0.5rem;
}

/* SVG Chart Card */
.chart-card {
  padding: 1.5rem;
  min-height: 280px;
}

.empty-chart-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: var(--text-secondary);
  text-align: center;
}

.empty-chart-state span {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.chart-content {
  display: grid;
  grid-template-columns: 180px 1fr;
  gap: 2rem;
  align-items: center;
}

.svg-container {
  position: relative;
  width: 100%;
}

.donut-svg {
  transform: rotate(-90deg);
}

.donut-bg {
  stroke: var(--border-glass);
  stroke-width: 10;
  fill: transparent;
}

.donut-segment {
  stroke-linecap: round;
  transition: stroke-dashoffset var(--transition-slow);
}

.donut-center-text {
  transform: rotate(90deg);
  transform-origin: center;
}

.donut-center-val {
  fill: var(--text-primary);
  font-size: 10px;
  font-weight: 700;
  text-anchor: middle;
}

.donut-center-lbl {
  fill: var(--text-secondary);
  font-size: 6px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  text-anchor: middle;
}

.chart-legend {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
  max-height: 200px;
  overflow-y: auto;
  padding-right: 0.5rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  padding: 0.5rem;
  border-radius: var(--radius-sm);
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
}

.legend-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
}

.legend-emoji {
  font-size: 1.1rem;
}

.legend-name {
  color: var(--text-primary);
  font-weight: 500;
  margin-right: auto;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.legend-value {
  color: var(--text-secondary);
  font-weight: 600;
  margin-right: 0.5rem;
}

.legend-pct {
  font-size: 0.75rem;
  background: var(--surface-3);
  padding: 0.1rem 0.3rem;
  border-radius: 4px;
  color: var(--text-secondary);
}

/* SECTION 2: INTERACTION GRID */
.interaction-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.form-card, .filters-card {
  padding: 1.5rem;
}

.purchase-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.select-control {
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='rgba(255,255,255,0.6)' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 1.2rem;
  padding-right: 2.5rem;
}

[data-theme="light"] .select-control {
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='rgba(0,0,0,0.45)' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
}

.select-control option {
  background: var(--bg-input);
  color: var(--text-primary);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 0.5rem;
}

.submit-btn {
  flex: 1;
}

/* Filters Custom UI */
.filters-container {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

/* Segmented Control / Tabs */
.filter-tabs {
  display: flex;
  background: var(--bg-tabs);
  border: 1px solid var(--border-glass);
  padding: 4px;
  border-radius: var(--radius-sm);
  margin-bottom: 0.5rem;
}

.tab-btn {
  flex: 1;
  background: transparent;
  border: none;
  color: var(--text-secondary);
  font-family: inherit;
  font-size: 0.85rem;
  font-weight: 500;
  padding: 0.6rem 0.8rem;
  border-radius: calc(var(--radius-sm) - 4px);
  cursor: pointer;
  outline: none;
  transition: all var(--transition-fast);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
}

.tab-btn:hover {
  color: var(--text-primary);
  background: var(--surface-1);
}

.tab-btn.active {
  color: #fff;
  background: linear-gradient(135deg, var(--primary) 0%, hsl(263, 80%, 55%) 100%);
  box-shadow: 0 2px 10px hsla(263, 80%, 62%, 0.2);
}


.clear-filters-btn {
  width: 100%;
  padding: 0.75rem;
}

/* SECTION 3: LIST SECTION */
.list-section {
  padding: 1.5rem;
}

.list-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.list-header .card-title {
  margin-bottom: 0;
  border-bottom: none;
  padding-bottom: 0;
}

.count-badge {
  font-size: 0.8rem;
  font-weight: 600;
  padding: 0.35rem 0.75rem;
  border-radius: 99px;
  background: var(--surface-3);
  border: 1px solid var(--border-glass);
  color: var(--text-secondary);
}

.table-container {
  overflow-x: auto;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-glass);
}

.empty-table-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  color: var(--text-secondary);
  text-align: center;
}

.empty-table-state span {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.purchase-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.purchase-table th {
  background: var(--surface-1);
  padding: 1rem;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-secondary);
  border-bottom: 1px solid var(--border-glass);
}

.purchase-table td {
  padding: 1rem;
  font-size: 0.95rem;
  border-bottom: 1px solid var(--border-glass);
  color: var(--text-primary);
  vertical-align: middle;
}

.table-row {
  transition: background-color var(--transition-fast);
}

.table-row:hover {
  background-color: var(--row-hover);
}

.category-tag {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.8rem;
  font-weight: 500;
  padding: 0.35rem 0.75rem;
  border-radius: 6px;
  color: var(--text-primary);
}

.tag-icon {
  font-size: 1rem;
}

.text-right { text-align: right !important; }
.text-center { text-align: center !important; }

.col-actions {
  width: 100px;
}

.action-btn {
  background: transparent;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  padding: 0.25rem;
  margin: 0 0.25rem;
  border-radius: 4px;
  transition: all var(--transition-fast);
}

.action-btn:hover {
  background: var(--surface-4);
  transform: scale(1.1);
}

/* SECTION 4: MODALS & OVERLAYS */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: var(--overlay-bg);
  backdrop-filter: blur(8px);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.modal-content {
  width: 100%;
  max-width: 650px;
  background: var(--bg-card);
  border: 1px solid var(--border-glass);
  box-shadow: var(--shadow-glass);
  border-radius: var(--radius-md);
  padding: 2rem;
  position: relative;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid var(--border-glass);
  padding-bottom: 0.75rem;
}

.modal-header h2 {
  font-size: 1.4rem;
  font-weight: 600;
  color: var(--text-primary);
}

.close-modal-btn {
  background: transparent;
  border: none;
  font-size: 2rem;
  cursor: pointer;
  color: var(--text-secondary);
  line-height: 1;
}

.close-modal-btn:hover {
  color: var(--text-primary);
}

.modal-body {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.modal-form-box {
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
  padding: 1.25rem;
}

.modal-form-box h4 {
  font-size: 0.95rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  margin-bottom: 0.75rem;
  letter-spacing: 0.05em;
}

.form-row-cat {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.form-row-cat .input-control {
  flex: 2;
}

.form-row-cat .icon-input {
  flex: 1;
  text-align: center;
}

.form-row-cat .color-input {
  width: 45px;
  height: 40px;
  padding: 2px;
  cursor: pointer;
  flex-shrink: 0;
}

.cat-form-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-sm {
  padding: 0.5rem 1rem;
  font-size: 0.85rem;
}

.modal-cat-list h4 {
  font-size: 0.95rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  margin-bottom: 0.75rem;
  letter-spacing: 0.05em;
}

.cat-list-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 200px;
  overflow-y: auto;
  padding-right: 0.25rem;
}

.cat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.65rem 1rem;
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
}

.cat-item-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.cat-item-icon {
  font-size: 1.2rem;
}

.cat-item-name {
  font-weight: 500;
}

.cat-item-color-label {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-family: monospace;
}

.cat-item-actions {
  display: flex;
  align-items: center;
}

/* RESPONSIVE DESIGN */
@media (max-width: 900px) {
  .dashboard-grid, .interaction-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .stats-sidebar {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1rem;
    align-items: stretch;
  }

  .manage-categories-btn {
    grid-column: span 3;
    margin-top: 0;
  }

  .chart-content {
    grid-template-columns: 1fr;
    justify-items: center;
    gap: 1.5rem;
  }
  
  .chart-legend {
    width: 100%;
    max-height: none;
  }
}

@media (max-width: 720px) {
  .stats-sidebar {
    grid-template-columns: 1fr;
  }

  .manage-categories-btn {
    grid-column: span 1;
  }
}

@media (max-width: 600px) {
  .form-row-cat {
    flex-direction: column;
    align-items: stretch;
    gap: 0.85rem;
  }

  .form-row-cat .input-control,
  .form-row-cat .icon-input {
    width: 100%;
    flex: none;
  }

  .form-row-cat .color-input {
    width: 100%;
    height: 44px;
    flex-shrink: 0;
  }

  .cat-form-actions {
    display: flex;
    gap: 0.5rem;
    width: 100%;
  }

  .cat-form-actions .btn {
    flex: 1;
  }

  .modal-content {
    padding: 1.5rem;
  }
}

@media (max-width: 500px) {
  .purchase-table th, 
  .purchase-table td {
    padding: 0.65rem 0.5rem;
    font-size: 0.82rem;
  }

  .category-tag {
    padding: 0.25rem 0.5rem;
    font-size: 0.75rem;
  }

  .tag-icon {
    font-size: 0.85rem;
  }

  .action-btn {
    font-size: 0.9rem;
    padding: 0.15rem;
    margin: 0 0.15rem;
  }
}

@media (max-width: 400px) {
  .form-row {
    grid-template-columns: 1fr;
    gap: 0.85rem;
  }
}
</style>
