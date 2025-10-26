# 🧾 EstoqueRolos — Sistema de Controle de Estoque de Bobinas

Sistema desktop desenvolvido em **C# (WPF)** com **SQLite**, voltado para o **gerenciamento de estoque de bobinas e rolos industriais**.  
O projeto foi criado para automatizar e simplificar o controle de entrada, saída e cálculo de metragem das bobinas, substituindo o processo manual sujeito a erros humanos.

---

## Funcionalidades Principais

- **Cadastro de Bobinas/Rolos** com informações como:
  - Código
  - Tipo de material
  - Metragem total e restante
  - Peso e dimensões

- **Controle de Saída e Consumo**
  - Atualização automática da metragem restante após cada uso.
  - Prevenção de valores negativos (estoque mínimo).

- **Relatórios e Filtros**
  - Pesquisa dinâmica por código, tipo, data ou status.
  - Exibição organizada em tabela com colunas ordenáveis.

- **Backup e Restauração**
  - Exportação e importação do banco de dados `.sqlite`.
  - Rotina de salvamento automático para evitar perda de dados.

---

## 🧰 Tecnologias Utilizadas

| Categoria | Tecnologia |
|------------|-------------|
| Linguagem  | C# (.NET 6 / WPF) |
| Banco de Dados | SQLite (via `Microsoft.Data.Sqlite`) |
| Interface | XAML (Windows Presentation Foundation) |
| Padrão de Projeto | MVVM simplificado |
| Controle de Tempo | `System.Timers` |
| Serialização / I/O | `System.IO`, `ObservableCollection` |

---

## ⚙️ Como Executar o Projeto

### Pré-requisitos:
- [.NET 6 SDK ou superior](https://dotnet.microsoft.com/download)
- Windows 10 ou superior




