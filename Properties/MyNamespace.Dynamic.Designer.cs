using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using GenieClient.Mapper;
using Microsoft.VisualBasic;

namespace GenieClient.My
{
    [System.CodeDom.Compiler.GeneratedCode("MyTemplate", "11.0.0.0")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal partial class MyApplication : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
    }

    [System.CodeDom.Compiler.GeneratedCode("MyTemplate", "11.0.0.0")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal partial class MyComputer : Microsoft.VisualBasic.Devices.Computer
    {
        [DebuggerHidden()]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public MyComputer() : base()
        {
        }
    }

    [HideModuleName()]
    [System.CodeDom.Compiler.GeneratedCode("MyTemplate", "11.0.0.0")]
    internal static class MyProject
    {
        [System.ComponentModel.Design.HelpKeyword("My.Computer")]
        internal static MyComputer Computer
        {
            [DebuggerHidden()]
            get
            {
                return m_ComputerObjectProvider.GetInstance;
            }
        }

        private readonly static ThreadSafeObjectProvider<MyComputer> m_ComputerObjectProvider = new ThreadSafeObjectProvider<MyComputer>();

        [System.ComponentModel.Design.HelpKeyword("My.Application")]
        internal static MyApplication Application
        {
            [DebuggerHidden()]
            get
            {
                return m_AppObjectProvider.GetInstance;
            }
        }

        private readonly static ThreadSafeObjectProvider<MyApplication> m_AppObjectProvider = new ThreadSafeObjectProvider<MyApplication>();

        [System.ComponentModel.Design.HelpKeyword("My.User")]
        internal static Microsoft.VisualBasic.ApplicationServices.User User
        {
            [DebuggerHidden()]
            get
            {
                return m_UserObjectProvider.GetInstance;
            }
        }

        private readonly static ThreadSafeObjectProvider<Microsoft.VisualBasic.ApplicationServices.User> m_UserObjectProvider = new ThreadSafeObjectProvider<Microsoft.VisualBasic.ApplicationServices.User>();

        [System.ComponentModel.Design.HelpKeyword("My.Forms")]
        internal static MyForms Forms
        {
            [DebuggerHidden()]
            get
            {
                return m_MyFormsObjectProvider.GetInstance;
            }
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
        internal sealed class MyForms
        {
            [DebuggerHidden()]
            private static T Create__Instance__<T>(T Instance) where T : Form, new()
            {
                if (Instance is null || Instance.IsDisposed)
                {
                    if (m_FormBeingCreated is object)
                    {
                        if (m_FormBeingCreated.ContainsKey(typeof(T)) == true)
                        {
                            throw new InvalidOperationException(Microsoft.VisualBasic.CompilerServices.Utils.GetResourceString("WinForms_RecursiveFormCreate"));
                        }
                    }
                    else
                    {
                        m_FormBeingCreated = new Hashtable();
                    }

                    m_FormBeingCreated.Add(typeof(T), null);
                    try
                    {
                        return new T();
                    }
                    catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException is object)
                    {
                        string BetterMessage = Microsoft.VisualBasic.CompilerServices.Utils.GetResourceString("WinForms_SeeInnerException", ex.InnerException.Message);
                        throw new InvalidOperationException(BetterMessage, ex.InnerException);
                    }
                    finally
                    {
                        m_FormBeingCreated.Remove(typeof(T));
                    }
                }
                else
                {
                    return Instance;
                }
            }

            [DebuggerHidden()]
            private void Dispose__Instance__<T>(ref T instance) where T : Form
            {
                instance.Dispose();
                instance = null;
            }

            [DebuggerHidden()]
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public MyForms() : base()
            {
            }

            [ThreadStatic()]
            private static Hashtable m_FormBeingCreated;

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override bool Equals(object o)
            {
                return base.Equals(o);
            }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            internal new Type GetType()
            {
                return typeof(MyForms);
            }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override string ToString()
            {
                return base.ToString();
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogChangelog m_DialogChangelog;

            public DialogChangelog DialogChangelog
            {
                [DebuggerHidden]
                get
                {
                    m_DialogChangelog = MyForms.Create__Instance__(m_DialogChangelog);
                    return m_DialogChangelog;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogChangelog)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogChangelog);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogConnect m_DialogConnect;

            public DialogConnect DialogConnect
            {
                [DebuggerHidden]
                get
                {
                    m_DialogConnect = MyForms.Create__Instance__(m_DialogConnect);
                    return m_DialogConnect;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogConnect)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogConnect);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogEdit m_DialogEdit;

            public DialogEdit DialogEdit
            {
                [DebuggerHidden]
                get
                {
                    m_DialogEdit = MyForms.Create__Instance__(m_DialogEdit);
                    return m_DialogEdit;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogEdit)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogEdit);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogException m_DialogException;

            public DialogException DialogException
            {
                [DebuggerHidden]
                get
                {
                    m_DialogException = MyForms.Create__Instance__(m_DialogException);
                    return m_DialogException;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogException)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogException);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogKey m_DialogKey;

            public DialogKey DialogKey
            {
                [DebuggerHidden]
                get
                {
                    m_DialogKey = MyForms.Create__Instance__(m_DialogKey);
                    return m_DialogKey;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogKey)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogKey);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogProfileConnect m_DialogProfileConnect;

            public DialogProfileConnect DialogProfileConnect
            {
                [DebuggerHidden]
                get
                {
                    m_DialogProfileConnect = MyForms.Create__Instance__(m_DialogProfileConnect);
                    return m_DialogProfileConnect;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogProfileConnect)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogProfileConnect);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogReconnect m_DialogReconnect;

            public DialogReconnect DialogReconnect
            {
                [DebuggerHidden]
                get
                {
                    m_DialogReconnect = MyForms.Create__Instance__(m_DialogReconnect);
                    return m_DialogReconnect;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogReconnect)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogReconnect);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogScriptName m_DialogScriptName;

            public DialogScriptName DialogScriptName
            {
                [DebuggerHidden]
                get
                {
                    m_DialogScriptName = MyForms.Create__Instance__(m_DialogScriptName);
                    return m_DialogScriptName;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogScriptName)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogScriptName);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogProfileNote m_DialogProfileNote;

            public DialogProfileNote DialogProfileNote
            {
                [DebuggerHidden]
                get
                {
                    m_DialogProfileNote = MyForms.Create__Instance__(m_DialogProfileNote);
                    return m_DialogProfileNote;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogProfileNote)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogProfileNote);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogSetClasses m_DialogSetClasses;

            public DialogSetClasses DialogSetClasses
            {
                [DebuggerHidden]
                get
                {
                    m_DialogSetClasses = MyForms.Create__Instance__(m_DialogSetClasses);
                    return m_DialogSetClasses;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogSetClasses)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogSetClasses);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogDragTarget m_DialogDragTarget;

            public DialogDragTarget DialogDragTarget
            {
                [DebuggerHidden]
                get
                {
                    m_DialogDragTarget = MyForms.Create__Instance__(m_DialogDragTarget);
                    return m_DialogDragTarget;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogDragTarget)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogDragTarget);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogUserWalk m_DialogUserWalk;

            public DialogUserWalk DialogUserWalk
            {
                [DebuggerHidden]
                get
                {
                    m_DialogUserWalk = MyForms.Create__Instance__(m_DialogUserWalk);
                    return m_DialogUserWalk;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogUserWalk)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogUserWalk);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public DialogSetTypeahead m_DialogSetTypeahead;

            public DialogSetTypeahead DialogSetTypeahead
            {
                [DebuggerHidden]
                get
                {
                    m_DialogSetTypeahead = MyForms.Create__Instance__(m_DialogSetTypeahead);
                    return m_DialogSetTypeahead;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_DialogSetTypeahead)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DialogSetTypeahead);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public FormConfig m_FormConfig;

            public FormConfig FormConfig
            {
                [DebuggerHidden]
                get
                {
                    m_FormConfig = MyForms.Create__Instance__(m_FormConfig);
                    return m_FormConfig;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_FormConfig)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_FormConfig);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public FormMain m_FormMain;

            public FormMain FormMain
            {
                [DebuggerHidden]
                get
                {
                    m_FormMain = MyForms.Create__Instance__(m_FormMain);
                    return m_FormMain;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_FormMain)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_FormMain);
                }
            }



            [EditorBrowsable(EditorBrowsableState.Never)]
            public ScriptExplorer m_ScriptExplorer;

            public ScriptExplorer ScriptExplorer
            {
                [DebuggerHidden]
                get
                {
                    m_ScriptExplorer = MyForms.Create__Instance__(m_ScriptExplorer);
                    return m_ScriptExplorer;
                }

                [DebuggerHidden]
                set
                {
                    if (value == m_ScriptExplorer)
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_ScriptExplorer);
                }
            }
        }

        private static ThreadSafeObjectProvider<MyForms> m_MyFormsObjectProvider = new ThreadSafeObjectProvider<MyForms>();

        [System.ComponentModel.Design.HelpKeyword("My.WebServices")]
        internal static MyWebServices WebServices
        {
            [DebuggerHidden()]
            get
            {
                return m_MyWebServicesObjectProvider.GetInstance;
            }
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
        internal sealed class MyWebServices
        {
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [DebuggerHidden()]
            public override bool Equals(object o)
            {
                return base.Equals(o);
            }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [DebuggerHidden()]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [DebuggerHidden()]
            internal new Type GetType()
            {
                return typeof(MyWebServices);
            }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [DebuggerHidden()]
            public override string ToString()
            {
                return base.ToString();
            }

            [DebuggerHidden()]
            private static T Create__Instance__<T>(T instance) where T : new()
            {
                if (instance is null)
                {
                    return new T();
                }
                else
                {
                    return instance;
                }
            }

            [DebuggerHidden()]
            private void Dispose__Instance__<T>(ref T instance)
            {
                instance = default;
            }

            [DebuggerHidden()]
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public MyWebServices() : base()
            {
            }
        }

        private readonly static ThreadSafeObjectProvider<MyWebServices> m_MyWebServicesObjectProvider = new ThreadSafeObjectProvider<MyWebServices>();

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Runtime.InteropServices.ComVisible(false)]
        internal sealed class ThreadSafeObjectProvider<T> where T : new()
        {
            internal T GetInstance
            {
                [DebuggerHidden()]
                get
                {
                    if (m_ThreadStaticValue is null)
                        m_ThreadStaticValue = new T();
                    return m_ThreadStaticValue;
                }
            }

            [DebuggerHidden()]
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public ThreadSafeObjectProvider() : base()
            {
            }

            [System.Runtime.CompilerServices.CompilerGenerated()]
            [ThreadStatic()]
            private static T m_ThreadStaticValue;
        }
    }
}
