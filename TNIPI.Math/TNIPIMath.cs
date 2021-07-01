using System;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;

namespace TNIPI.Math
{
    /// <summary>
    /// This class will control the lifecycle of the Module.
    /// The order of the methods are the same as the calling order.
    /// </summary>
    [ModuleAppearance(typeof(TNIPIMathAppearance))]
    public class TNIPIMath : IModule
    {
        public TNIPIMath()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region IModule Members

        /// <summary>
        /// This method runs once in the Module life; when it loaded into the petrel.
        /// This method called first.
        /// </summary>
        public void Initialize()
        {
            // TODO:  Add TNIPIMath.Initialize implementation
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the not UI related components.
        /// (eg: datasource, plugin)
        /// </summary>
        public void Integrate()
        {
            // Registrations:
            Sum sumInstance = new Sum();
            PetrelSystem.WorkflowEditor.Add(sumInstance); 

            Minimum minimumInstance = new Minimum();
            PetrelSystem.WorkflowEditor.Add(minimumInstance);

            Maximum maximumInstance = new Maximum();
            PetrelSystem.WorkflowEditor.Add(maximumInstance);

            ArithmeticAverage arithmeticaverageInstance = new ArithmeticAverage();
            PetrelSystem.WorkflowEditor.Add(arithmeticaverageInstance);

            HarmonicAverage harmonicaverageInstance = new HarmonicAverage();
            PetrelSystem.WorkflowEditor.Add(harmonicaverageInstance);

            GeometricAverage geometricaverageInstance = new GeometricAverage();
            PetrelSystem.WorkflowEditor.Add(geometricaverageInstance);

            StandardDeviation standarddeviationInstance = new StandardDeviation();
            PetrelSystem.WorkflowEditor.Add(standarddeviationInstance);

            MostOf mostofInstance = new MostOf();
            PetrelSystem.WorkflowEditor.Add(mostofInstance);

            // TODO:  Add TNIPIMath.Integrate implementation

        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the UI related components.
        /// (eg: settingspages, treeextensions)
        /// </summary>
        public void IntegratePresentation()
        {
            // Registrations:


            // TODO:  Add TNIPIMath.IntegratePresentation implementation
        }

        /// <summary>
        /// This method called once in the life of the module; 
        /// right before the module is unloaded. 
        /// It is usually when the application is closing.
        /// </summary>
        public void Disintegrate()
        {
            // TODO:  Add TNIPIMath.Disintegrate implementation
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO:  Add TNIPIMath.Dispose implementation
        }

        #endregion

    }

    #region ModuleAppearance Class

    /// <summary>
    /// Appearance (or branding) for a Slb.Ocean.Core.IModule.
    /// This is associated with a module using Slb.Ocean.Core.ModuleAppearanceAttribute.
    /// </summary>
    internal class TNIPIMathAppearance : IModuleAppearance
    {
        /// <summary>
        /// Description of the module.
        /// </summary>
        public string Description
        {
            get { return "Implementation providing operations on several grid properties for workflows"; }
        }

        /// <summary>
        /// Display name for the module.
        /// </summary>
        public string DisplayName
        {
            get { return "Workflow group operation plug-in"; }
        }

        /// <summary>
        /// Returns the name of a image resource.
        /// </summary>
        public string ImageResourceName
        {
            get { return null; }
        }

        /// <summary>
        /// A link to the publisher or null.
        /// </summary>
        public Uri ModuleUri
        {
            get { return null; }
        }
    }

    #endregion
}