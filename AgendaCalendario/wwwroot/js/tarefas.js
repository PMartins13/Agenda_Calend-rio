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
        const catId = tarefa.categoriaId ?? 'null'; // null para "Sem categoria"
        if (!categorias[catId]) {
            categorias[catId] = {
                id: tarefa.categoriaId,
                nome: tarefa.categoriaNome || 'Sem categoria',
                cor: tarefa.cor || '#ccc',
                tarefas: []
            };
        }
        categorias[catId].tarefas.push(tarefa);
    });

    for (const [_, info] of Object.entries(categorias)) {
        const id = info.id;
        const nome = info.nome;
        const cor = info.cor;
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

        bloco.innerHTML = `
            <h5 class="d-flex justify-content-between align-items-center" style="color:${cor};">
                ${nome}
                ${id !== null ? `
                    <span>
                        <button class="btn btn-sm btn-outline-primary me-2 btnEditarCategoria"
                                data-id="${id}" data-nome="${nome}" data-cor="${cor}">✏️</button>
                        <button class="btn btn-sm btn-outline-danger btnApagarCategoria"
                                data-id="${id}">🗑️</button>
                    </span>
                ` : ''}
            </h5>
        `;
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

function carregarCategoriasLista() {
    fetch("/Categorias/Minhas")
        .then(res => res.json())
        .then(categorias => {
            const lista = document.getElementById("listaCategorias");
            if (!lista) return;

            lista.innerHTML = "";
            categorias.forEach(cat => {
                const item = document.createElement("li");
                item.className = "list-group-item d-flex justify-content-between align-items-center";
                item.innerHTML = `
                    <span>
                        <span class="badge rounded-pill me-2" style="background-color: ${cat.cor};">&nbsp;</span>
                        ${cat.nome}
                    </span>
                    <span>
                        <button class="btn btn-sm btn-outline-primary me-2 btnEditarCategoria"
                            data-id="${cat.id}" data-nome="${cat.nome}" data-cor="${cat.cor}">✏️</button>
                        <button class="btn btn-sm btn-outline-danger btnApagarCategoria"
                            data-id="${cat.id}">🗑️</button>
                    </span>`;
                lista.appendChild(item);
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
            // Esta linha Ã© ESSENCIAL:
            document.getElementById("editTituloOriginal").value = tarefa.titulo;

            document.getElementById("editDataFimRecorrencia").value = tarefa.dataFimRecorrencia?.split("T")[0] || "";
            document.getElementById("divEditDataFimRecorrencia").style.display = tarefa.recorrencia && tarefa.recorrencia !== 0 ? "block" : "none";
            const editRecorrenciaInput = document.getElementById("editRecorrencia");
            if (editRecorrenciaInput) {
                editRecorrenciaInput.addEventListener("change", function () {
                    const divFim = document.getElementById("divEditDataFimRecorrencia");
                    divFim.style.display = this.value === "0" ? "none" : "block";
                });
            }
            carregarCategoriasDropdown("editCategoriaId");

            setTimeout(() => {
                document.getElementById("editCategoriaId").value = tarefa.categoriaId ?? "";
            }, 200);

            const modal = new bootstrap.Modal(document.getElementById("modalEditarTarefa"));
            modal.show();
        });
}

// Adicione esta função se ainda não existir:
function showConfirm(message, onConfirm) {
    $('#confirmModalMessage').text(message);
    $('#confirmModal').modal('show');
    $('#confirmModalOk').off('click').on('click', function () {
        $('#confirmModal').modal('hide');
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
    });
}

// Substitua todos os alert() por showError()
function showError(message) {
    $('#successToastMessage').text(message);
    var toast = new bootstrap.Toast(document.getElementById('successToast'));
    toast.show();
}

document.addEventListener('DOMContentLoaded', function () {
    carregarTarefas(dataSelecionada);
    carregarCategoriasLista();

    document.getElementById("recorrencia").addEventListener("change", function () {
        const divFim = document.getElementById("divDataFimRecorrencia");
        divFim.style.display = this.value === "0" ? "none" : "block";
    });

    const calendarEl = document.getElementById('calendar');
    let calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'pt',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: ''
        },
        events: function(fetchInfo, successCallback, failureCallback) {
            const inicio = fetchInfo.startStr.split("T")[0]; // só a parte yyyy-MM-dd
            const fim = fetchInfo.endStr.split("T")[0];

            fetch(`/Tarefas/PorIntervalo?inicio=${inicio}&fim=${fim}`)
                .then(response => {
                    if (!response.ok) throw new Error("Erro ao buscar dados.");
                    return response.json();
                })
                .then(data => successCallback(data))
                .catch(() => failureCallback());
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
            categoriaId: document.getElementById("categoriaId").value || null,
            recorrencia: parseInt(document.getElementById("recorrencia").value),
            dataFimRecorrencia: document.getElementById("dataFimRecorrencia").value || null
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
                showError("Erro ao criar tarefa");
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
                showError("Categoria criada com sucesso!");
                carregarCategoriasDropdown();
                carregarCategoriasLista();
            } else {
                showError("Erro ao criar categoria.");
            }
        });
    });

    document.getElementById("formEditarTarefa").addEventListener("submit", function (e) {
        e.preventDefault();

        const tarefa = {
            id: document.getElementById("editId").value,
            titulo: document.getElementById("editTitulo").value,
            descricao: document.getElementById("editDescricao").value,
            data: document.getElementById("editData").value,
            categoriaId: document.getElementById("editCategoriaId").value || null,
            recorrencia: parseInt(document.getElementById("editRecorrencia").value),
            dataFimRecorrencia: document.getElementById("editDataFimRecorrencia").value || null
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
                showError("Erro ao editar tarefa.");
            }
        });
    });

    document.getElementById("btnApagarTarefa").addEventListener("click", () => {
        const id = document.getElementById("editId").value;

        showConfirm("Tens a certeza que queres eliminar esta tarefa?", function () {
            fetch("/Tarefas/Apagar", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(id)
            }).then(res => {
                if (res.ok) {
                    bootstrap.Modal.getInstance(document.getElementById("modalEditarTarefa")).hide();
                    carregarTarefas(dataSelecionada);
                } else {
                    showError("Erro ao apagar tarefa.");
                }
            });
        });
    });

    document.getElementById("btnApagarTodasTarefas").addEventListener("click", () => {
        const titulo = document.getElementById("editTitulo").value;

        showConfirm("Tens a certeza que queres eliminar TODAS as tarefas com este título?", function () {
            fetch("/Tarefas/ApagarTodasComTitulo", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(titulo)
            }).then(res => {
                if (res.ok) {
                    bootstrap.Modal.getInstance(document.getElementById("modalEditarTarefa")).hide();
                    carregarTarefas(dataSelecionada);
                } else {
                    showError("Erro ao apagar todas as tarefas.");
                }
            });
        });
    });

    document.getElementById("btnGuardarTodasTarefas").addEventListener("click", () => {
        const tarefa = {
            tituloOriginal: document.getElementById("editTituloOriginal").value,
            titulo: document.getElementById("editTitulo").value,
            descricao: document.getElementById("editDescricao").value,
            data: document.getElementById("editData").value,
            categoriaId: document.getElementById("editCategoriaId").value || null,
            recorrencia: parseInt(document.getElementById("editRecorrencia").value),
            dataFimRecorrencia: document.getElementById("editDataFimRecorrencia").value || null
        };

        fetch("/Tarefas/EditarTodasComTitulo", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(tarefa)
        }).then(res => {
            if (res.ok) {
                bootstrap.Modal.getInstance(document.getElementById("modalEditarTarefa")).hide();
                carregarTarefas(dataSelecionada);
            } else {
                showError("Erro ao editar todas as tarefas.");
            }
        });
    });

    document.getElementById("formEditarCategoria").addEventListener("submit", function (e) {
        e.preventDefault();

        const categoriaEditada = {
            id: document.getElementById("modalEditCategoriaId").value,
            nome: document.getElementById("modalEditCategoriaNome").value,
            cor: document.getElementById("modalEditCategoriaCor").value
        };

        fetch("/Categorias/Editar", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(categoriaEditada)
        }).then(res => {
            if (res.ok) {
                bootstrap.Modal.getInstance(document.getElementById("modalEditarCategoria")).hide();
                carregarCategoriasLista();
                carregarCategoriasDropdown();
            } else {
                showError("Erro ao editar categoria.");
            }
        });
    });

    document.getElementById("painelTarefas")?.addEventListener("click", function (e) {
        const target = e.target;

        if (target.classList.contains("btnEditarCategoria")) {
            document.getElementById("modalEditCategoriaId").value = target.dataset.id;
            document.getElementById("modalEditCategoriaNome").value = target.dataset.nome;
            document.getElementById("modalEditCategoriaCor").value = target.dataset.cor;

            const modal = new bootstrap.Modal(document.getElementById("modalEditarCategoria"));
            modal.show();
        }

        if (target.classList.contains("btnApagarCategoria")) {
            const id = target.dataset.id;

            showConfirm("Tens a certeza que queres eliminar esta categoria?", function () {
                fetch("/Categorias/Eliminar", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(id)
                }).then(res => {
                    if (res.ok) {
                        carregarCategoriasDropdown(); // opcional
                        carregarTarefas(dataSelecionada); // recarregar vista
                    } else {
                        res.text().then(msg => showError(msg));
                    }
                });
            });
        }
    });

})
