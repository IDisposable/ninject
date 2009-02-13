﻿using System;
using System.Collections.Generic;
using Ninject.Activation.Hooks;
using Ninject.Infrastructure.Disposal;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Activation.Scope
{
	/// <summary>
	/// A scope used for deterministic disposal of activated instances. When the scope is
	/// disposed, all instances activated via it will be deactivated.
	/// </summary>
	public class ActivationScope : DisposableObject, IActivationScope
	{
		/// <summary>
		/// Gets or sets the parent resolution root (usually the kernel).
		/// </summary>
		public IResolutionRoot Parent { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationScope"/> class.
		/// </summary>
		/// <param name="parent">The parent resolution root.</param>
		public ActivationScope(IResolutionRoot parent)
		{
			Parent = parent;
		}

		/// <summary>
		/// Determines whether the specified request can be resolved.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
		public bool CanResolve(IRequest request)
		{
			return Parent.CanResolve(request);
		}

		/// <summary>
		/// Resolves the specified request.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraints">The constraints to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <returns>A series of hooks that can be used to resolve instances that match the request.</returns>
		public IEnumerable<IHook> Resolve(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters)
		{
			return Resolve(CreateDirectRequest(service, constraints, parameters));
		}

		/// <summary>
		/// Resolves the specified request.
		/// </summary>
		/// <param name="request">The request to resolve.</param>
		/// <returns>A series of hooks that can be used to resolve instances that match the request.</returns>
		public IEnumerable<IHook> Resolve(IRequest request)
		{
			return Parent.Resolve(request);
		}

		/// <summary>
		/// Creates a request for the specified service.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraints">The constraints to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <returns>The created request.</returns>
		protected virtual IRequest CreateDirectRequest(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters)
		{
			return new Request(service, constraints, parameters, () => this);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.Get(serviceType);
		}
	}
}