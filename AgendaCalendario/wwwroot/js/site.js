/**
 * Documentação principal do módulo de gestão de interações com o utilizador
 * Este ficheiro contém funções para manipulação de modais, toasts e eventos de botões
 * Requer jQuery e Bootstrap para funcionar corretamente
 */

/**
 * Função que apresenta uma modal de confirmação ao utilizador
 * @param {string} message - Mensagem a ser apresentada na modal
 * @param {Function} onConfirm - Função de callback a ser executada quando o utilizador confirmar
 */
function showConfirm(message, onConfirm) {
    // Define a mensagem na modal de confirmação
    $('#confirmModalMessage').text(message);
    // Apresenta a modal
    $('#confirmModal').modal('show');
    
    // Remove eventos anteriores e adiciona novo evento ao botão de confirmação
    $('#confirmModalOk').off('click').on('click', function () {
        // Esconde a modal após confirmação
        $('#confirmModal').modal('hide');
        showConfirm();
    });
}

/**
 * Função que apresenta uma notificação toast de sucesso
 * @param {string} message - Mensagem de sucesso a ser apresentada
 */
function showSuccess(message) {
    // Define a mensagem no toast
    $('#successToastMessage').text(message);
    // Cria uma nova instância do toast do Bootstrap
    var toast = new bootstrap.Toast(document.getElementById('successToast'));
    // Apresenta o toast
    toast.show();
}

// Gestão de Eventos dos Botões

/**
 * Evento de clique no botão de eliminar tarefa individual
 * Apresenta confirmação antes de proceder à eliminação
 */
$('#btnApagarTarefa').on('click', function () {
    showConfirm('Tem a certeza que deseja eliminar esta tarefa?', function () {
        // TODO: Implementar chamada AJAX para eliminar a tarefa
        showSuccess('Tarefa eliminada com sucesso!');
    });
});

/**
 * Evento de clique no botão de eliminar todas as tarefas
 * Apresenta confirmação antes de proceder à eliminação em massa
 */
$('#btnApagarTodasTarefas').on('click', function () {
    showConfirm('Tem a certeza que deseja eliminar todas as tarefas?', function () {
        // TODO: Implementar chamada AJAX para eliminar todas as tarefas
        showSuccess('Todas as tarefas foram eliminadas!');
    });
});

/**
 * Evento de submissão do formulário
 * Apresenta uma notificação de sucesso após guardar
 */
$('form').on('submit', function (e) {
    // e.preventDefault(); // Descomenta esta linha para prevenir a submissão normal do formulário
    showSuccess('Tarefa guardada com sucesso!');
});

// Nota: Este código assume a existência dos seguintes elementos no HTML:
// - Modal com ID 'confirmModal'
// - Elemento com ID 'confirmModalMessage' dentro da modal
// - Botão com ID 'confirmModalOk' dentro da modal
// - Toast com ID 'successToast'
// - Elemento com ID 'successToastMessage' dentro do toast