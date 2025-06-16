let dataSelecionada = new Date().toISOString().split("T")[0]; // hoje

function carregarTarefas(data) {
    fetch(`/Tarefas/PorData?data=${data}`)
        .then(res => res.json())
        .then(tarefas => {
            mostrarTarefas(tarefas);
        });
}

function mostrarTarefas(tarefas) {
    const painel = document.getElementById('painelTarefas');
    painel.innerHTML = '';

    if (tarefas.length === 0) {
        painel.innerHTML = '<p>Sem tarefas para este dia.</p>';
        return;
    }

    // Agrupar tarefas por categoria
    const categorias = {};

    tarefas.forEach(tarefa => {
        const cat = tarefa.categoriaNome || 'Sem categoria';
        if (!categorias[cat]) categorias[cat] = [];
        categorias[cat].push(tarefa);
    });

    for (const [categoria, lista] of Object.entries(categorias)) {
        const bloco = document.createElement('div');
        bloco.className = 'mb-3 p-2 border rounded';

        bloco.innerHTML = `
            <h5>${categoria}</h5>
            <ul>
                ${lista.map(t => `
                    <li>
                        <strong>${t.titulo}</strong><br/>
                        <small>${t.descricao || ''}</small>
                    </li>
                `).join('')}
            </ul>

        `;

        painel.appendChild(bloco);
    }
}

// Inicializar com o dia de hoje
document.addEventListener('DOMContentLoaded', function () {
    carregarTarefas(dataSelecionada);

    const calendarEl = document.getElementById('calendar');
    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'pt',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: ''
        },
        dateClick: function (info) {
            dataSelecionada = info.dateStr;
            carregarTarefas(dataSelecionada);

            // Remover destaque anterior
            document.querySelectorAll('.fc-daygrid-day').forEach(el => {
                el.classList.remove('selected-day');
            });

            // Adicionar destaque ao dia clicado
            info.dayEl.classList.add('selected-day');
        }

    });

    calendar.render();
});

document.getElementById("btnAdicionarTarefa").addEventListener("click", () => {
    document.getElementById("formTarefa").reset();
    document.getElementById("data").value = dataSelecionada;
    const modal = new bootstrap.Modal(document.getElementById("modalTarefa"));
    modal.show();
});

document.getElementById("formTarefa").addEventListener("submit", function (e) {
    e.preventDefault();

    const tarefa = {
        titulo: document.getElementById("titulo").value,
        descricao: document.getElementById("descricao").value,
        data: document.getElementById("data").value
    };

    fetch("/Tarefas/Criar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(tarefa)
    })
        .then(res => {
            if (res.ok) {
                bootstrap.Modal.getInstance(document.getElementById("modalTarefa")).hide();
                carregarTarefas(dataSelecionada);
            } else {
                alert("Erro ao criar tarefa");
            }
        });
});

