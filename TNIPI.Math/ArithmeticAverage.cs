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
    /// This class contains all the methods and subclasses of the ArithmeticAverage.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    public class ArithmeticAverage : Workstep<ArithmeticAverage.Arguments>, IPresentation, IDescriptionSource
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

            Invoke_ArithmeticAverage(args.Grid, args.PropertyCollection, args.OutputProperty);
        }

        private void Invoke_ArithmeticAverage(Grid grid, PropertyCollection propertyCollection, Property outputProperty)
        {
            PetrelLogger.InfoOutputWindow("Invoke_ArithmeticAverage start: " + DateTime.Now.ToString());

            using (ITransaction trans = DataManager.NewTransaction(Thread.CurrentThread))
            {
                trans.Lock(outputProperty);

                FastPropertyIndexer outfpi = outputProperty.SpecializedAccess.OpenFastPropertyIndexer();
                Dictionary<Property, FastPropertyIndexer> fpiCol =
                    new Dictionary<Property, FastPropertyIndexer>();

                foreach (Property prop in propertyCollection.Properties)
                {
                    FastPropertyIndexer fpi = prop.SpecializedAccess.OpenFastPropertyIndexer();
                    fpiCol.Add(prop, fpi);
                }

                for (int i = 0; i < grid.NumCellsIJK.I; i++)
                    for (int j = 0; j < grid.NumCellsIJK.J; j++)
                        for (int k = 0; k < grid.NumCellsIJK.K; k++)
                        {
                            bool areAllValuesDefined = true;
                            double sum = 0.0;

                            foreach (Property prop in propertyCollection.Properties)
                            {
                                FastPropertyIndexer fpi = fpiCol[prop];

                                float value = fpi[i, j, k];
                                if (value == float.NaN)
                                {
                                    areAllValuesDefined = false;
                                    break;
                                }

                                sum += value;
                            }

                            if (!areAllValuesDefined)
                            {
                                outfpi[i, j, k] = float.NaN;
                                continue;
                            }

                            outfpi[i, j, k] = (float)(sum / propertyCollection.PropertyCount);
                        } 
                
                trans.Commit();
            }

            PetrelLogger.InfoOutputWindow("Invoke_ArithmeticAverage end: " + DateTime.Now.ToString());
        }

        private bool CheckInputArguments(Arguments args)
        {
            foreach (DescribedArgument arg in args.DescribedArguments)
                if (!IsArgumentValueNull(arg))
                    return false;

            if (args.PropertyCollection.PropertyCount < 2)
            {
                throw new WorkstepException("Less than two properties found in " + args.PropertyCollection.Description.Name);
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
        /// ArgumentPackage class for ArithmeticAverage.
        /// Each public property is an argument in the package.  The name, type and
        /// input/output role are taken from the property and modified by any
        /// attributes applied.
        /// </summary>
        public class Arguments : DescribedArgumentsByReflection
        {
            private Slb.Ocean.Petrel.DomainObject.PillarGrid.Grid grid;
            private Slb.Ocean.Petrel.DomainObject.PillarGrid.Property outputProperty;
            private Slb.Ocean.Petrel.DomainObject.PillarGrid.PropertyCollection propertyCollection;

            [TakeValueOfWith3DGrid]
            [Description("Grid", "3D grid")]
            public Slb.Ocean.Petrel.DomainObject.PillarGrid.Grid Grid
            {
                internal get { return this.grid; }
                set { this.grid = value; }
            }

            [Description("Output", "Output continuous property")]
            public Slb.Ocean.Petrel.DomainObject.PillarGrid.Property OutputProperty
            {
                get { return this.outputProperty; }
                set { this.outputProperty = value; }
            }

            [Description("Property collection", "Discrete/continuous property collection")]
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
        /// Gets the description of the ArithmeticAverage
        /// </summary>
        public IDescription Description
        {
            get { return ArithmeticAverageDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the ArithmeticAverage.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class ArithmeticAverageDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private  static ArithmeticAverageDescription instance = new ArithmeticAverageDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static ArithmeticAverageDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of ArithmeticAverage
            /// </summary>
            public string Name
            {
                get { return "Arithmetic mean"; }
            }
            /// <summary>
            /// Gets the short description of ArithmeticAverage
            /// </summary>
            public string ShortDescription
            {
                get { return "Generate single continuous property representing aritmetic mean from a range of properties."; }
            }
            /// <summary>
            /// Gets the detailed description of ArithmeticAverage
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