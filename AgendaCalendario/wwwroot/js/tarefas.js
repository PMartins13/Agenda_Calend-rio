let dataSelecionada = new Date().toISOString().split("T")[0];

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

    const categorias = {};

    tarefas.forEach(tarefa => {
        const cat = tarefa.categoriaNome || 'Sem categoria';
        if (!categorias[cat]) categorias[cat] = { cor: tarefa.cor, tarefas: [] };
        categorias[cat].tarefas.push(tarefa);
    });

    for (const [categoria, info] of Object.entries(categorias)) {
        const cor = info.cor || '#cccccc';
        const lista = info.tarefas;

        const bloco = document.createElement('div');
        bloco.className = 'mb-3 p-2 border rounded';
        bloco.style.borderLeft = `8px solid ${cor}`;
        bloco.style.backgroundColor = cor + '10'; // cor com transparência leve
        bloco.style.transition = 'all 0.3s ease';

        bloco.innerHTML = `
            <h5 style="color:${cor};">${categoria}</h5>
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


function carregarCategoriasDropdown() {
    fetch("/Categorias/Minhas")
        .then(res => res.json())
        .then(categorias => {
            const select = document.getElementById("categoriaId");
            select.innerHTML = '<option value="">(Sem categoria)</option>';

            categorias.forEach(cat => {
                const opt = document.createElement("option");
                opt.value = cat.id;
                opt.textContent = cat.nome;
                select.appendChild(opt);
            });
        });
}

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
            document.querySelectorAll('.fc-daygrid-day').forEach(el => {
                el.classList.remove('selected-day');
            });
            info.dayEl.classList.add('selected-day');
        }
    });

    calendar.render();
});

document.getElementById("btnAdicionarTarefa").addEventListener("click", () => {
    document.getElementById("formTarefa").reset();
    document.getElementById("data").value = dataSelecionada;
    carregarCategoriasDropdown();
    const modal = new bootstrap.Modal(document.getElementById("modalTarefa"));
    modal.show();
});

document.getElementById("formTarefa").addEventListener("submit", function (e) {
    e.preventDefault();

    const tarefa = {
        titulo: document.getElementById("titulo").value,
        descricao: document.getElementById("descricao").value,
        data: document.getElementById("data").value,
        categoriaId: document.getElementById("categoriaId").value || null
    };

    fetch("/Tarefas/Criar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(tarefa)
    }).then(res => {
        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById("modalTarefa")).hide();
            carregarTarefas(dataSelecionada);
        } else {
            alert("Erro ao criar tarefa");
        }
    });
});

document.getElementById("btnCriarCategoria").addEventListener("click", () => {
    document.getElementById("formCategoria").reset();
    const modal = new bootstrap.Modal(document.getElementById("modalCategoria"));
    modal.show();
});

document.getElementById("formCategoria").addEventListener("submit", function (e) {
    e.preventDefault();

    const novaCategoria = {
        nome: document.getElementById("categoriaNome").value,
        cor: document.getElementById("categoriaCor").value
    };

    fetch("/Categorias/Criar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(novaCategoria)
    }).then(res => {
        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById("modalCategoria")).hide();
            alert("Categoria criada com sucesso!");
            carregarCategoriasDropdown();
        } else {
            alert("Erro ao criar categoria.");
        }
    });
});




