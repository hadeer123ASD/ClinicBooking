(function () {
    const doctorSelect = document.getElementById('Form_DoctorId');
    const dateInput = document.getElementById('Form_AppointmentDate');
    const slotsBox = document.getElementById('slots');

    if (!doctorSelect || !dateInput || !slotsBox) {
        return;
    }

    let preselected = slotsBox.dataset.selected || '';
    let lastRequest = 0;

    function showMessage(text, cssClass) {
        slotsBox.innerHTML = '';
        const span = document.createElement('span');
        span.className = cssClass;
        span.textContent = text;
        slotsBox.appendChild(span);
    }

    function showSlots(slots) {
        slotsBox.innerHTML = '';
        const list = document.createElement('div');
        list.className = 'slots-list';

        slots.forEach(function (slot) {
            const id = 'slot-' + slot.value.replace(':', '');

            const input = document.createElement('input');
            input.type = 'radio';
            input.className = 'btn-check';
            input.name = 'Form.StartTime';
            input.id = id;
            input.value = slot.value;
            input.required = true;
            input.checked = slot.value === preselected;

            const label = document.createElement('label');
            label.className = 'btn btn-outline-primary';
            label.htmlFor = id;
            label.textContent = slot.text;

            list.appendChild(input);
            list.appendChild(label);
        });

        slotsBox.appendChild(list);
    }

    async function loadSlots() {
        const doctorId = doctorSelect.value;
        const date = dateInput.value;

        if (!doctorId || !date) {
            showMessage('Select a doctor and a date to see the free times.', 'text-muted small');
            return;
        }

        const requestId = ++lastRequest;
        showMessage('Loading available times...', 'text-muted small');

        try {
            const url = slotsBox.dataset.url + '?doctorId=' + encodeURIComponent(doctorId) +
                '&date=' + encodeURIComponent(date);
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error('Request failed');
            }

            const data = await response.json();

            // Ignore the response if the doctor or date changed in the meantime
            if (requestId !== lastRequest) {
                return;
            }

            if (data.slots.length === 0) {
                showMessage(data.message || 'No free time slots on this date.', 'text-danger small');
                return;
            }

            showSlots(data.slots);
        } catch (error) {
            if (requestId === lastRequest) {
                showMessage('Could not load the available times. Please try again.', 'text-danger small');
            }
        }
    }

    function onSelectionChanged() {
        preselected = '';
        loadSlots();
    }

    doctorSelect.addEventListener('change', onSelectionChanged);
    dateInput.addEventListener('change', onSelectionChanged);

    // After a failed save the form is shown again with the old values
    loadSlots();
})();
