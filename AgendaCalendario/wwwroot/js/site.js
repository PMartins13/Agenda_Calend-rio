// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Função para mostrar modal de confirmação
function showConfirm(message, onConfirm) {
    $('#confirmModalMessage').text(message);
    $('#confirmModal').modal('show');
    $('#confirmModalOk').off('click').on('click', function () {
        $('#confirmModal').modal('hide');
        showConfirm();
    });
}

// Função para mostrar toast de sucesso
function showSuccess(message) {
    $('#successToastMessage').text(message);
    var toast = new bootstrap.Toast(document.getElementById('successToast'));
    toast.show();
}

// Botão "Eliminar"
$('#btnApagarTarefa').on('click', function () {
    showConfirm('Tem a certeza que deseja eliminar esta tarefa?', function () {
        // Código AJAX para eliminar a tarefa
        showSuccess('Tarefa eliminada com sucesso!');
    });
});

// Botão "Eliminar Todas"
$('#btnApagarTodasTarefas').on('click', function () {
    showConfirm('Tem a certeza que deseja eliminar todas as tarefas?', function () {
        // Código AJAX para eliminar todas as tarefas
        showSuccess('Todas as tarefas foram eliminadas!');
    });
});

// Botão "Guardar" (exemplo de toast após guardar)
$('form').on('submit', function (e) {
    // e.preventDefault(); // Descomente se quiser evitar o submit real
    showSuccess('Tarefa guardada com sucesso!');
});
