using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Генератор_баз
{
    class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<string> ComboList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Login> ComboListLogin { get; set; } = new ObservableCollection<Login>();
        public ObservableCollection<string> ComboListPass { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Thread> Threads { get; set; } = new ObservableCollection<Thread>();

        public ObservableCollection<Prop> PropLogin { get; set; } = new ObservableCollection<Prop>
        {
            new Prop(false, "Логин пользователя без цифр", Funcs.delNumbers),
            new Prop(false, "Логин пользователя без букв", Funcs.delChars),
            new Prop(false, "Логин содержащий только буквы и цифры, без спец символов", Funcs.delSpecChars),
            new Prop(false, "Все буквы содержащиеся в логине переводятся в верхний регистр", Funcs.toUp),
            new Prop(false, "Все буквы содержащиеся в логине переводятся внижний регистр", Funcs.toLower),
            new Prop(false, "Логин в обратном порядке", Funcs.reverse)
        };

        public ObservableCollection<Prop> PropPass { get; set; } = new ObservableCollection<Prop>
        {
            new Prop(false, "Пароль пользователя без цифр", Funcs.delNumbers),
            new Prop(false, "Пароль пользователя без букв", Funcs.delChars),
            new Prop(false, "Пароль содержащий только буквы и цифры, без спец символов", Funcs.delSpecChars),
            new Prop(false, "Все буквы содержащиеся в пароле переводятся в верхний регистр", Funcs.toUp),
            new Prop(false, "Все буквы содержащиеся в пароле переводятся внижний регистр", Funcs.toLower),
            new Prop(false, "Пароль в обратном порядке", Funcs.reverse)
        };

        private string _LoginPath;
        public string LoginPath
        {
            get => _LoginPath;
            set
            {
                _LoginPath = value;
                OnPropertyChanged();
            }
        }

        private string _PassPath;
        public string PassPath
        {
            get => _PassPath;
            set
            {
                _PassPath = value;
                OnPropertyChanged();
            }
        }

        private string _TimeLess = "00:00:00";
        public string TimeLess
        {
            get => _TimeLess;
            set
            {
                _TimeLess = value;
                OnPropertyChanged();
            }
        }

        private int _ComboCreated;
        public int ComboCreated
        {
            get => _ComboCreated;
            set
            {
                _ComboCreated = value;
                OnPropertyChanged();
            }
        }

        private int _ComboCreatedLogin;
        public int ComboCreatedLogin
        {
            get => _ComboCreatedLogin;
            set
            {
                _ComboCreatedLogin = value;
                OnPropertyChanged();
            }
        }

        private int _ComboCreatedPass;
        public int ComboCreatedPass
        {
            get => _ComboCreatedPass;
            set
            {
                _ComboCreatedPass = value;
                OnPropertyChanged();
            }
        }

        private int _ThreadCount = 5;
        public int ThreadCount
        {
            get => _ThreadCount;
            set
            {
                if (value <= 0) value = 1;
                _ThreadCount = value;
                OnPropertyChanged();
            }
        }

        private bool _isStart;
        public bool isStart
        {
            get => _isStart;
            set
            {
                _isStart = value;
                OnPropertyChanged();
            }
        }

        private bool _useDomains;
        public bool useDomains
        {
            get => _useDomains;
            set
            {
                _useDomains = value;
                OnPropertyChanged();
            }
        }

        private string _DomainList = "gmail.com yandex.ru mail.ru";
        public string DomainList
        {
            get => _DomainList;
            set
            {
                _DomainList = value;
                OnPropertyChanged();
            }
        }

        private ICommand _LoadLoginCommand;
        public ICommand LoadLoginCommand => _LoadLoginCommand ?? (_LoadLoginCommand = new RelayCommand(o =>
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == true)
            {
                LoginPath = open.FileName;
            }
        }));

        private string s_time = "";
        private DateTime time = DateTime.Now;

        private string[] domains = null;

        private ICommand _StartCommand;
        public ICommand StartCommand => _StartCommand ?? (_StartCommand = new RelayCommand(async o =>
        {
            isStart = true;

            ComboList.Clear();
            ComboListLogin.Clear();
            ComboListPass.Clear();
            Threads.Clear();
            ComboCreatedPass = ComboCreated = ComboCreatedLogin = 0;

            time = DateTime.Now;
            s_time = time.ToString().Replace(":", ".");

            if (!File.Exists(LoginPath))
            {
                MessageBox.Show("База данных не найдена!");
                isStart = false;
                return;
            }

            format[] l = PropLogin.Where(q => q.Active).Select(q => q.Format).ToArray();
            format[] p = PropPass.Where(q => q.Active).Select(q => q.Format).ToArray();

            if (useDomains)
                domains = DomainList.Split(' ');

            await Task.Factory.StartNew(() =>
            {
                using (StreamReader sr = new StreamReader(LoginPath, Encoding.UTF8))
                {
                    string s = "";
                    for (s = sr.ReadLine(); s != null; s = sr.ReadLine())
                    {
                        string[] s_d = s.Split(':');
                        string full_login = s_d[0];
                        string pass = s_d[1];
                        string[] l_i = full_login.Split('@');
                        string login_name = l_i[0];
                        string domain = l_i[1];

                        ComboListLogin.Add(new Login(login_name, domain));
                        ComboListPass.Add(pass);
                        ComboCreatedLogin++;
                        ComboCreatedPass++;

                        foreach (format f in l)
                        {
                            string r = f(login_name);
                            if (r != login_name && !string.IsNullOrWhiteSpace(r))
                            {
                                ComboListLogin.Add(new Login(r, domain));
                                ComboCreatedLogin++;
                            }
                        }

                        foreach (format f in p)
                        {
                            string r = f(pass);
                            if (r != pass && !string.IsNullOrWhiteSpace(r))
                            {
                                ComboListPass.Add(r);
                                ComboCreatedPass++;
                            }
                        }
                    }
                }

                for (int i = 0; i < ThreadCount; i++)
                {
                    Thread t = new Thread(work);
                    Threads.Add(t);
                    t.IsBackground = true;
                    t.Start();
                }
            });

            Thread check = new Thread(w =>
            {
                while (true)
                {
                    if (Threads.Count == 0)
                    {
                        SaveToFile();
                        MessageBox.Show("Готово!");
                        return;
                    }
                    TimeLess = (DateTime.Now - time).ToString(@"hh\:mm\:ss");
                    Thread.Sleep(1000);
                }
            });
            check.IsBackground = true;
            check.Start();

        }, o => !isStart));

        private object new_comb = new object();
        private object thread = new object();

        private void work()
        {
            Login login = null;
            while ((login = getLogin()) != null && _isStart)
            {
                foreach (string pass in ComboListPass)
                    lock (new_comb)
                        addToComb(login, pass);
            }

            //lock (thread)
            Threads.Remove(Thread.CurrentThread);
        }

        private object save = new object();
        private object saveToFile = new object();

        private void SaveToFile()
        {
            lock (saveToFile)
            {
                using (StreamWriter sw = new StreamWriter("result-" + s_time + ".txt", true, Encoding.UTF8))
                {
                    while (ComboList.Count != 0)
                    {
                        string comb = ComboList[0];
                        ComboList.RemoveAt(0);
                        sw.WriteLine(comb);
                    }
                }
            }
        }

        private void addToComb(Login login, string pass)
        {

            if (useDomains)
                foreach (string d in domains)
                {
                    ComboList.Add(login.login + '@' + d + ':' + pass);
                    ComboCreated++;
                }
            else
            {
                ComboList.Add(login.login + '@' + login.base_domain + ':' + pass);
                ComboCreated++;
            }
            //lock (save)
            //{
                if (ComboList.Count >= 10000)
                {
                    SaveToFile();
                }
            //}
        }

        private object login = new object();

        private Login getLogin()
        {

            if (ComboListLogin.Count == 0)
            {
                isStart = false;
                return null;
            };
            lock (login)
            {
                Login l = ComboListLogin[0];
                ComboListLogin.RemoveAt(0);
                return l;
            }
        }

        private ICommand _StopCommand;
        public ICommand StopCommand => _StopCommand ?? (_StopCommand = new RelayCommand(o =>
        {
            foreach (Thread t in Threads)
                t.Abort();

            Threads.Clear();

            isStart = false;
        }, o => isStart));
    }
}
