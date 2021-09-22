using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest
        {
            public ActivityDto Activity { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                //var activity = await _context.Activities.AsNoTracking()
                //    .AnyAsync(x => x.Id == request.Activity.Id);

                //var dest = _mapper.Map<Activity>(request.Activity);
                //_context.Update(dest);
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                if (activity == null)
                    throw new Exception("Could not find activity");
                var dest = _mapper.Map<Activity>(request.Activity);
                _mapper.Map(dest, activity);                
                var success = await _context.SaveChangesAsync() > 0;
                if (success)
                    return Unit.Value;
                throw new Exception("Problem saving changes");
            }
        }
    }
}
