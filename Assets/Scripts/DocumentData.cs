using UnityEngine;

public class DocumentData : MonoBehaviour
{
    // Enum para facilitar a escolha do tipo no Inspector
    public enum DocumentType { Passaporte, HistoricoAcademico, CartaDeAceite, CertificadoIdioma, SeguroSaude }

    [Header("Informações do Documento")]
    public DocumentType tipoDeDocumento;
    public string nomeDoAluno = "Fulano de Tal";
    public bool eValido = true; // Define se este documento é genuíno ou falso/vencido

    // Você pode adicionar mais variáveis conforme a necessidade
    // public string dataDeValidade;
    // public string paisDeOrigem;
}