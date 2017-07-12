<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Models.ErrorQuestion>>" %>

    <table>
        <tr>
            <th>
                题号
            </th>
            <th>
                正确答案
            </th>
            <th>
                你的答案
            </th>
            <th>
                解析
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%: item.QuestionNum %>
            </td>
            <td>
                <%: item.SAnswer %>
            </td>
            <td>
                <%: item.UAnswer %>
            </td>
            <td>
                <%: item.Analyze %>
            </td>
        </tr>
    
    <% } %>

    </table>


