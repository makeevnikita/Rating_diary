using System.Text.RegularExpressions;

namespace Diary
{
    //Класс, который проверяет поля на символы
    class Validation
    {
        public bool EmptyVariable(string variable)//Пустая переменная или нет
        {
            if (variable == "")
                return true;
            else
                return false;
        }

        public bool SpaceInWord(string variable)//Пробелы в переменной
        {
            Regex regex = new Regex(@"\s");

            if (regex.IsMatch(variable))
                return true;
            else
                return false;
        }

        public bool SpecSymbol(string variable)//Спецсимволы в переменной
        {
            Regex specSymbols = new Regex(@"[ !@#$%^&*()_+|}{:?>< ]");
            if (specSymbols.IsMatch(variable))
            {
                return true;
            }
            return false;
        }

        public bool Integer(string variable)//Целоечисленная переменная
        {
            if (!int.TryParse(variable, out int n))
            {
                return true;
            }
            return false;
        }
    }
}
