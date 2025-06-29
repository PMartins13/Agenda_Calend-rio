/**
 * Variável global para armazenar a data atualmente selecionada
 * Inicializada com a data atual no formato ISO (YYYY-MM-DD)
 */
let dataSelecionada = new Date().toISOString().split("T")[0];

/**
 * Carrega as tarefas para uma data específica através de uma chamada AJAX
 * @param {string} data - Data no formato YYYY-MM-DD
 */
function carregarTarefas(data) {
    fetch(`/Tarefas/PorData?data=${data}`)
        .then(res => res.json())
        .then(tarefas => {
            mostrarTarefas(tarefas);
        });
}

/**
 * Apresenta as tarefas no painel, organizadas por categorias
 * @param {Array} tarefas - Array de objetos com as tarefas
 */
function mostrarTarefas(tarefas) {
    const painel = document.getElementById('painelTarefas');
    painel.innerHTML = '';

    // Verifica se existem tarefas para apresentar
    if (tarefas.length === 0) {
        painel.innerHTML = '<p>Sem tarefas para este dia.</p>';
        return;
    }

    // Objeto para agrupar tarefas por categoria
    const categorias = {};

    // Organiza as tarefas por categorias
    tarefas.forEach(tarefa => {
        // Processa tarefas com categorias
        tarefa.categorias.forEach(cat => {
            if (!categorias[cat.id]) {
                categorias[cat.id] = {
                    id: cat.id,
                    nome: cat.nome,
                    cor: cat.cor,
                    tarefas: []
                };
            }
            categorias[cat.id].tarefas.push(tarefa);
        });

        // Processa tarefas sem categoria
        if (!tarefa.categorias.length) {
            if (!categorias.null) {
                categorias.null = {
                    id: null,
                    nome: 'Sem categoria',
                    cor: '#ccc',
                    tarefas: []
                };
            }
            categorias.null.tarefas.push(tarefa);
        }
    });

    // Cria blocos visuais para cada categoria
    for (const [_, info] of Object.entries(categorias)) {
        const id = info.id;
        const nome = info.nome;
        const cor = info.cor;
        const lista = info.tarefas;

        // Cria o contentor para a categoria
        const bloco = document.createElement('div');
        bloco.className = 'mb-3 p-2 border rounded';
        bloco.style.borderLeft = `8px solid ${cor}`;
        bloco.style.backgroundColor = cor + '10';

        // Cria a lista de tarefas
        const ul = document.createElement('ul');

        // Adiciona cada tarefa à lista
        lista.forEach(t => {
            const li = document.createElement('li');
            li.innerHTML = `<strong>${t.titulo}</strong><br/><small>${t.descricao || ''}</small>`;
            li.style.cursor = "pointer";
            li.addEventListener("dblclick", () => abrirModalEditar(t.id));
            ul.appendChild(li);
        });

        // Adiciona o cabeçalho com os botões de gestão
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

/**
 * Carrega as categorias disponíveis para um elemento select
 * @param {string} targetSelectId - ID do elemento select a ser preenchido
 */
function carregarCategoriasDropdown(targetSelectId = "categoriaId") {
    fetch("/Categorias/Minhas")
        .then(res => res.json())
        .then(categorias => {
            const select = document.getElementById(targetSelectId);
            select.innerHTML = '';

            // Preenche o select com as categorias
            categorias.forEach(cat => {
                const opt = document.createElement("option");
                opt.value = cat.id;
                opt.textContent = cat.nome;
                select.appendChild(opt);
            });

            // Configura o select para seleção múltipla
            select.setAttribute("multiple", true);
        });
}

/**
 * Carrega e apresenta a lista de categorias na interface
 * Inclui botões para editar e eliminar cada categoria
 */
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

/**
 * Abre o modal de edição de tarefa com os dados carregados
 * @param {number} tarefaId - ID da tarefa a ser editada
 */
function abrirModalEditar(tarefaId) {
    fetch(`/Tarefas/Detalhes?id=${tarefaId}`)
        .then(res => res.json())
        .then(tarefa => {
            // Preenche os campos do formulário com os dados da tarefa
            document.getElementById("editId").value = tarefa.id;
            document.getElementById("editTitulo").value = tarefa.titulo;
            document.getElementById("editDescricao").value = tarefa.descricao;
            document.getElementById("editData").value = tarefa.data.split("T")[0];
            document.getElementById("editTituloOriginal").value = tarefa.titulo;

            // Configura campos de recorrência
            document.getElementById("editDataFimRecorrencia").value = tarefa.dataFimRecorrencia?.split("T")[0] || "";
            document.getElementById("divEditDataFimRecorrencia").style.display = 
                tarefa.recorrencia && tarefa.recorrencia !== 0 ? "block" : "none";

            // Configura o evento de alteração da recorrência
            const editRecorrenciaInput = document.getElementById("editRecorrencia");
            if (editRecorrenciaInput) {
                editRecorrenciaInput.addEventListener("change", function () {
                    const divFim = document.getElementById("divEditDataFimRecorrencia");
                    divFim.style.display = this.value === "0" ? "none" : "block";
                });
            }

            // Carrega as categorias e seleciona as associadas à tarefa
            carregarCategoriasDropdown("editCategoriaId");
            setTimeout(() => {
                const select = document.getElementById("editCategoriaId");
                Array.from(select.options).forEach(opt => {
                    opt.selected = tarefa.categoriasIds.includes(parseInt(opt.value));
                });
            }, 200);
            
            // Mostra o modal
            const modal = new bootstrap.Modal(document.getElementById("modalEditarTarefa"));
            modal.show();
        });
}

/**
 * Apresenta um modal de confirmação personalizado
 * @param {string} message - Mensagem a ser apresentada
 * @param {Function} onConfirm - Função a ser executada após confirmação
 */
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

/**
 * Apresenta uma mensagem de erro em formato toast
 * @param {string} message - Mensagem de erro a ser apresentada
 */
function showError(message) {
    $('#successToastMessage').text(message);
    var toast = new bootstrap.Toast(document.getElementById('successToast'));
    toast.show();
}

/**
 * Configuração inicial e gestão de eventos quando o DOM estiver carregado
 */
document.addEventListener('DOMContentLoaded', function () {
    // Inicialização
    carregarTarefas(dataSelecionada);
    carregarCategoriasLista();

    // Configuração do campo de recorrência
    document.getElementById("recorrencia").addEventListener("change", function () {
        const divFim = document.getElementById("divDataFimRecorrencia");
        divFim.style.display = this.value === "0" ? "none" : "block";
    });

    // Configuração do calendário FullCalendar
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
            const inicio = fetchInfo.startStr.split("T")[0];
            const fim = fetchInfo.endStr.split("T")[0];

            // Carrega eventos do período selecionado
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
            // Atualiza visual do dia selecionado
            document.querySelectorAll('.fc-daygrid-day').forEach(el => {
                el.classList.remove('selected-day');
            });
            info.dayEl.classList.add('selected-day');
        }
    });
    calendar.render();

    /**
     * Evento de clique no botão de adicionar tarefa
     * Prepara e mostra o modal de nova tarefa
     */
    document.getElementById("btnAdicionarTarefa").addEventListener("click", () => {
        document.getElementById("formTarefa").reset();
        document.getElementById("data").value = dataSelecionada;
        carregarCategoriasDropdown();
        const modal = new bootstrap.Modal(document.getElementById("modalTarefa"));
        modal.show();
    });

    /**
     * Gestão do formulário de criação de tarefa
     */
    document.getElementById("formTarefa").addEventListener("submit", function (e) {
        e.preventDefault();

        const tarefa = {
            titulo: document.getElementById("titulo").value,
            descricao: document.getElementById("descricao").value,
            data: document.getElementById("data").value,
            categoriasIds: Array.from(document.getElementById("categoriaId").selectedOptions)
                            .map(opt => parseInt(opt.value)),
            recorrencia: parseInt(document.getElementById("recorrencia").value),
            dataFimRecorrencia: document.getElementById("dataFimRecorrencia").value || null
        };

        // Envia pedido para criar nova tarefa
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

    /**
     * Gestão do botão e modal de criação de categoria
     */
    document.getElementById("btnCriarCategoria").addEventListener("click", () => {
        document.getElementById("formCategoria").reset();
        const modal = new bootstrap.Modal(document.getElementById("modalCategoria"));
        modal.show();
    });

    /**
     * Gestão do formulário de criação de categoria
     */
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

    /**
     * Gestão do formulário de edição de tarefa
     */
    document.getElementById("formEditarTarefa").addEventListener("submit", function (e) {
        e.preventDefault();

        const tarefa = {
            id: document.getElementById("editId").value,
            titulo: document.getElementById("editTitulo").value,
            descricao: document.getElementById("editDescricao").value,
            data: document.getElementById("editData").value,
            categoriasIds: Array.from(document.getElementById("editCategoriaId").selectedOptions)
                            .map(opt => parseInt(opt.value)),
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

    /**
     * Gestão do botão de eliminar tarefa
     */
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

    /**
     * Gestão do botão de eliminar todas as tarefas com o mesmo título
     */
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

    /**
     * Gestão do botão de guardar alterações em todas as tarefas com o mesmo título
     */
    document.getElementById("btnGuardarTodasTarefas").addEventListener("click", () => {
        const tarefa = {
            tituloOriginal: document.getElementById("editTituloOriginal").value,
            titulo: document.getElementById("editTitulo").value,
            descricao: document.getElementById("editDescricao").value,
            data: document.getElementById("editData").value,
            categoriasIds: Array.from(document.getElementById("editCategoriaId").selectedOptions)
                            .map(opt => parseInt(opt.value)),
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

    /**
     * Gestão do formulário de edição de categoria
     */
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

    /**
     * Gestão de eventos delegados para o painel de tarefas
     * Lida com botões de editar e eliminar categorias
     */
    document.getElementById("painelTarefas")?.addEventListener("click", function (e) {
        const target = e.target;

        // Gestão do botão de editar categoria
        if (target.classList.contains("btnEditarCategoria")) {
            document.getElementById("modalEditCategoriaId").value = target.dataset.id;
            document.getElementById("modalEditCategoriaNome").value = target.dataset.nome;
            document.getElementById("modalEditCategoriaCor").value = target.dataset.cor;

            const modal = new bootstrap.Modal(document.getElementById("modalEditarCategoria"));
            modal.show();
        }

        // Gestão do botão de eliminar categoria
        if (target.classList.contains("btnApagarCategoria")) {
            const id = target.dataset.id;

            showConfirm("Tens a certeza que queres eliminar esta categoria?", function () {
                fetch("/Categorias/Eliminar", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(id)
                }).then(res => {
                    if (res.ok) {
                        carregarCategoriasDropdown(); // Atualiza dropdowns
                        carregarTarefas(dataSelecionada); // Recarrega vista
                    } else {
                        res.text().then(msg => showError(msg));
                    }
                });
            });
        }
    });

}); // Fim do DOMContentLoaded