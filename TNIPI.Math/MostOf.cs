using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject;
using Slb.Ocean.Petrel.DomainObject.PillarGrid;

namespace TNIPI.Math
{
    /// <summary>
    /// This class contains all the methods and subclasses of the MostOf.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    public class MostOf : Workstep<MostOf.Arguments>, IPresentation, IDescriptionSource
    {
        /// <summary>
        /// This method does the work of the process.
        /// </summary>
        /// <param name="argumentPackage">the arguments to use during the process</param>
        protected override void InvokeSimpleCore(Arguments argumentPackage)
        {
            // TODO: finish the Invoke method implementation
            Arguments args = argumentPackage;
            if (!CheckInputArguments(args))
                return;

            Invoke_MostOf(args.Grid, args.PropertyCollection, args.OutputProperty);
        }
        /*
        private void Invoke_MostOf(Grid grid, PropertyCollection propertyCollection, DictionaryProperty outputProperty)
        {
            if (outputProperty.Facies.Count == 0)
                throw new WorkstepException("Facies code dictionary empty"); 

            PetrelLogger.InfoOutputWindow("Invoke_MostOf start: " + DateTime.Now.ToString());

            using (ITransaction trans = DataManager.NewTransaction(Thread.CurrentThread))
            {
                trans.Lock(outputProperty);

                int[] arrcnt = new int[outputProperty.Facies.Count];

                for (int i = 0; i < grid.NumCellsIJK.I; i++)
                    for (int j = 0; j < grid.NumCellsIJK.J; j++)
                        for (int k = 0; k < grid.NumCellsIJK.K; k++)
                        {
                            bool AreAllValuesDefined = true;

                            for (int n = 0; n < arrcnt.Length; arrcnt[n++] = 0);

                            foreach (DictionaryProperty dicProp in propertyCollection.DictionaryProperties)
                            {
                                if (dicProp[i, j, k] == DictionaryProperty.UndefinedValue)
                                {
                                    AreAllValuesDefined = false;
                                    break;
                                }

                                if (dicProp[i, j, k] >= arrcnt.Length)
                                    throw new WorkstepException("Facies code out of range at ("
                                        + i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ")");

                                arrcnt[dicProp[i, j, k]]++;
                            }

                            if (!AreAllValuesDefined)
                            {
                                outputProperty[i, j, k] = DictionaryProperty.UndefinedValue;
                                continue;
                            }

                            int max = 0, maxn = 0;
                            for (int n = 0; n < arrcnt.Length; n++)
                                if (n == 0 || arrcnt[n] > max)
                                {
                                    max = arrcnt[n];
                                    maxn = n;
                                }

                            outputProperty[i, j, k] = maxn;
                        }

                trans.Commit();
            }
 
            PetrelLogger.InfoOutputWindow("Invoke_MostOf end: " + DateTime.Now.ToString());
        }
        */
        /*
        private void Invoke_MostOf(Grid grid, PropertyCollection propertyCollection, DictionaryProperty outputProperty)
        {
            if (outputProperty.Facies.Count == 0)
                throw new WorkstepException("Facies code dictionary empty");

            PetrelLogger.InfoOutputWindow("Invoke_MostOf start: " + DateTime.Now.ToString());

            using (ITransaction trans = DataManager.NewTransaction(Thread.CurrentThread))
            {
                trans.Lock(outputProperty);

                int[] arrcnt = new int[outputProperty.Facies.Count];
                Dictionary<DictionaryProperty, FastDictionaryPropertyIndexer> fdpiCol =
                    new Dictionary<DictionaryProperty, FastDictionaryPropertyIndexer>();
                foreach (DictionaryProperty dicProp in propertyCollection.DictionaryProperties)
                {
                    FastDictionaryPropertyIndexer fdpi = dicProp.SpecializedAccess.OpenFastDictionaryPropertyIndexer();
                    fdpiCol.Add(dicProp, fdpi);
                }

                for (int i = 0; i < grid.NumCellsIJK.I; i++)
                    for (int j = 0; j < grid.NumCellsIJK.J; j++)
                        for (int k = 0; k < grid.NumCellsIJK.K; k++)
                        {
                            bool AreAllValuesDefined = true;

                            for (int n = 0; n < arrcnt.Length; arrcnt[n++] = 0) ;

                            foreach (DictionaryProperty dicProp in propertyCollection.DictionaryProperties)
                            {
                                if (dicProp[i, j, k] == DictionaryProperty.UndefinedValue)
                                {
                                    AreAllValuesDefined = false;
                                    break;
                                }

                                FastDictionaryPropertyIndexer fdpi = fdpiCol[dicProp];
                                if (fdpi[i, j, k] >= arrcnt.Length)
                                    throw new WorkstepException("Facies code out of range at ("
                                        + i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ")");

                                arrcnt[fdpi[i, j, k]]++;
                            }

                            if (!AreAllValuesDefined)
                            {
                                outputProperty[i, j, k] = DictionaryProperty.UndefinedValue;
                                continue;
                            }

                            int max = 0, maxn = 0;
                            for (int n = 0; n < arrcnt.Length; n++)
                                if (n == 0 || arrcnt[n] > max)
                                {
                                    max = arrcnt[n];
                                    maxn = n;
                                }

                            outputProperty[i, j, k] = maxn;
                        }

                trans.Commit();
            }

            PetrelLogger.InfoOutputWindow("Invoke_MostOf end: " + DateTime.Now.ToString());
        }
        */
        
        private void Invoke_MostOf(Grid grid, PropertyCollection propertyCollection, DictionaryProperty outputProperty)
        {
            PetrelLogger.InfoOutputWindow("Invoke_MostOf start: " + DateTime.Now.ToString());

            using (ITransaction trans = DataManager.NewTransaction(Thread.CurrentThread))
            {
                trans.Lock(outputProperty);

                //int[] arrcnt = new int[outputProperty.Facies.Count];
                ArrayList codeList = new ArrayList(outputProperty.Facies.Count);
                ArrayList cntList = new ArrayList(outputProperty.Facies.Count);

                FastDictionaryPropertyIndexer outfdpi = outputProperty.SpecializedAccess.OpenFastDictionaryPropertyIndexer();
                Dictionary<DictionaryProperty, FastDictionaryPropertyIndexer> fdpiCol =
                    new Dictionary<DictionaryProperty, FastDictionaryPropertyIndexer>();

                foreach (DictionaryProperty dicProp in propertyCollection.DictionaryProperties)
                {
                    FastDictionaryPropertyIndexer fdpi = dicProp.SpecializedAccess.OpenFastDictionaryPropertyIndexer();
                    fdpiCol.Add(dicProp, fdpi);
                }

                for (int i = 0; i < grid.NumCellsIJK.I; i++)
                    for (int j = 0; j < grid.NumCellsIJK.J; j++)
                        for (int k = 0; k < grid.NumCellsIJK.K; k++)
                        {
                            bool areAllValuesDefined = true;
                            //bool firstRun = true;

                            //for (int n = 0; n < cntList.Count; n++)
                            //{
                            //    int value = 0;
                            //    cntList[n] = value;
                            //}

                            foreach (DictionaryProperty dicProp in propertyCollection.DictionaryProperties)
                            {
                                FastDictionaryPropertyIndexer fdpi = fdpiCol[dicProp];

                                int value = fdpi[i, j, k];
                                if (value == DictionaryProperty.UndefinedValue)
                                {
                                    areAllValuesDefined = false;
                                    break;
                                }

                                //if (fdpi[i, j, k] >= arrcnt.Length)
                                //    throw new WorkstepException("Facies code out of range at ("
                                //        + i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ")");

                                //arrcnt[fdpi[i, j, k]]++;

                                int ind = codeList.IndexOf(value);
                                if (codeList.Count == 0 || ind == -1)
                                {
                                    codeList.Add(value);
                                    cntList.Add(1);
                                }
                                else
                                {
                                    int cnt = (int)cntList[ind] + 1;
                                    cntList[ind] = cnt;
                                }
                            }

                            if (!areAllValuesDefined)
                            {
                                outfdpi[i, j, k] = DictionaryProperty.UndefinedValue;

                                for (int n = 0; n < cntList.Count; n++)
                                {
                                    int value = 0;
                                    cntList[n] = value;
                                }

                                continue;
                            }

                            int maxcnt = 0, maxcode = 0;
                            for (int n = 0; n < cntList.Count; n++)
                            {
                                if (n == 0 || (int)cntList[n] > maxcnt)
                                {
                                    maxcnt = (int)cntList[n];
                                    maxcode = (int)codeList[n];
                                }

                                int value = 0;
                                cntList[n] = value;
                            }

                            outfdpi[i, j, k] = maxcode;
                        }

                trans.Commit();
            }

            PetrelLogger.InfoOutputWindow("Invoke_MostOf end: " + DateTime.Now.ToString());
        }
               
        private bool CheckInputArguments(Arguments args)
        {
            foreach (DescribedArgument arg in args.DescribedArguments)
                if (!IsArgumentValueNull(arg))
                    return false;

            if (args.OutputProperty.Facies.Count == 0)
            {
                throw new WorkstepException("Facies code dictionary empty");
                return false;
            }

            if (args.PropertyCollection.DictionaryPropertyCount == 0)
            {
                throw new WorkstepException("No discrete properties found in " + args.PropertyCollection.Description.Name);
                return false;
            }

            DictionaryPropertyVersion outDicPropVer = args.OutputProperty.DictionaryPropertyVersion;
            foreach(DictionaryProperty dicProp in args.PropertyCollection.DictionaryProperties)
                if (dicProp.DictionaryPropertyVersion != outDicPropVer)
                {
                    throw new WorkstepException("Property collection and output property templates do not match");
                    return false;
                }

            return true;
        }

        private bool IsArgumentValueNull(DescribedArgument arg)
        {
            if (arg.Value == null)
            {
                throw new WorkstepException(arg.Description.Name + " not provided"); 
                return false;
            }

            return true;
        }

        #region CopyArgPack implementation

        protected override void CopyArgumentPackageCore(Arguments fromArgumentPackage, Arguments toArgumentPackage)
        {
            DescribedArgumentsHelper.Copy(fromArgumentPackage, toArgumentPackage);
        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for MostOf.
        /// Each public property is an argument in the package.  The name, type and
        /// input/output role are taken from the property and modified by any
        /// attributes applied.
        /// </summary>
        public class Arguments : DescribedArgumentsByReflection
        {
            private Slb.Ocean.Petrel.DomainObject.PillarGrid.Grid grid;
            private Slb.Ocean.Petrel.DomainObject.PillarGrid.DictionaryProperty outputProperty; 
            private Slb.Ocean.Petrel.DomainObject.PillarGrid.PropertyCollection propertyCollection;


            [TakeValueOfWith3DGrid]
            [Description("Grid", "3D grid")]
            public Slb.Ocean.Petrel.DomainObject.PillarGrid.Grid Grid
            {
                internal get { return this.grid; }
                set { this.grid = value; }
            }

            [Description("Output", "Output discrete property")]
            public Slb.Ocean.Petrel.DomainObject.PillarGrid.DictionaryProperty OutputProperty
            {
                get { return this.outputProperty; }
                set { this.outputProperty = value; }
            }

            [Description("Property collection", "Discrete property collection")]
            public Slb.Ocean.Petrel.DomainObject.PillarGrid.PropertyCollection PropertyCollection
            {
                internal get { return this.propertyCollection; }
                set { this.propertyCollection = value; }
            }
        }
    
        #region IPresentation Members

        public event EventHandler PresentationChanged;

        public string Text
        {
            get { return Description.Name; }
        }

        public System.Drawing.Bitmap Image
        {
            get { return PetrelImages.Modules; }
        }

        #endregion

        #region IDescriptionSource Members

        /// <summary>
        /// Gets the description of the MostOf
        /// </summary>
        public IDescription Description
        {
            get { return MostOfDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the MostOf.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class MostOfDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private  static MostOfDescription instance = new MostOfDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static MostOfDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of MostOf
            /// </summary>
            public string Name
            {
                get { return "Most of"; }
            }
            /// <summary>
            /// Gets the short description of MostOf
            /// </summary>
            public string ShortDescription
            {
                get { return "Generate single discrete property representing most of from a range of properties."; }
            }
            /// <summary>
            /// Gets the detailed description of MostOf
            /// </summary>
            public string Description
            {
                get { return "The command with 3D grid must be run prior to the process commands in order to specify the grid to run the processes on. It is not automatically using the active grid."; }
            }

            #endregion
        }
        #endregion


    }
}