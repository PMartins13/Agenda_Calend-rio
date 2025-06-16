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
        bloco.style.backgroundColor = cor + '10';

        const ul = document.createElement('ul');

        lista.forEach(t => {
            const li = document.createElement('li');
            li.innerHTML = `<strong>${t.titulo}</strong><br/><small>${t.descricao || ''}</small>`;
            li.style.cursor = "pointer";
            li.addEventListener("dblclick", () => abrirModalEditar(t.id));
            ul.appendChild(li);
        });

        bloco.innerHTML = `<h5 style="color:${cor};">${categoria}</h5>`;
        bloco.appendChild(ul);
        painel.appendChild(bloco);
    }
}

function carregarCategoriasDropdown(targetSelectId = "categoriaId") {
    fetch("/Categorias/Minhas")
        .then(res => res.json())
        .then(categorias => {
            const select = document.getElementById(targetSelectId);
            select.innerHTML = '<option value="">(Sem categoria)</option>';

            categorias.forEach(cat => {
                const opt = document.createElement("option");
                opt.value = cat.id;
                opt.textContent = cat.nome;
                select.appendChild(opt);
            });
        });
}

function abrirModalEditar(tarefaId) {
    fetch(`/Tarefas/Detalhes?id=${tarefaId}`)
        .then(res => res.json())
        .then(tarefa => {
            document.getElementById("editId").value = tarefa.id;
            document.getElementById("editTitulo").value = tarefa.titulo;
            document.getElementById("editDescricao").value = tarefa.descricao;
            document.getElementById("editData").value = tarefa.data.split("T")[0];
            carregarCategoriasDropdown("editCategoriaId");

            setTimeout(() => {
                document.getElementById("editCategoriaId").value = tarefa.categoriaId ?? "";
            }, 200);

            const modal = new bootstrap.Modal(document.getElementById("modalEditarTarefa"));
            modal.show();
        });
}

// Guardar edição
document.getElementById("formEditarTarefa").addEventListener("submit", function (e) {
    e.preventDefault();

    const tarefa = {
        id: document.getElementById("editId").value,
        titulo: document.getElementById("editTitulo").value,
        descricao: document.getElementById("editDescricao").value,
        data: document.getElementById("editData").value,
        categoriaId: document.getElementById("editCategoriaId").value || null
    };

    fetch("/Tarefas/Editar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(tarefa)
    }).then(res => {
        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById("modalEditarTarefa")).hide();
            carregarTarefas(dataSelecionada);
        } else {
            alert("Erro ao editar tarefa.");
        }
    });
});

// Apagar tarefa
document.getElementById("btnApagarTarefa").addEventListener("click", () => {
    const id = document.getElementById("editId").value;

    if (!confirm("Tens a certeza que queres eliminar esta tarefa?")) return;

    fetch("/Tarefas/Apagar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    }).then(res => {
        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById("modalEditarTarefa")).hide();
            carregarTarefas(dataSelecionada);
        } else {
            alert("Erro ao apagar tarefa.");
        }
    });
});

// Resto dos eventos
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
