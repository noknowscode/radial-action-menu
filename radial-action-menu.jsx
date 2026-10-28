import { useState, useCallback } from 'react';

/**
 * RadialActionMenu - A floating action button that expands into a radial menu
 * 
 * @param {Object} props
 * @param {Array} props.items - Menu items [{icon, label, onClick, color?}]
 * @param {string} props.position - 'bottom-right' | 'bottom-left' | 'top-right' | 'top-left'
 * @param {string} props.accentColor - Primary accent color (CSS color value)
 * @param {number} props.radius - Distance of items from center (px)
 * @param {number} props.startAngle - Starting angle in degrees
 * @param {number} props.endAngle - Ending angle in degrees
 */

// Default icons as SVG components
const Icons = {
  Plus: () => (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
      <line x1="12" y1="5" x2="12" y2="19" />
      <line x1="5" y1="12" x2="19" y2="12" />
    </svg>
  ),
  Close: () => (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
      <line x1="18" y1="6" x2="6" y2="18" />
      <line x1="6" y1="6" x2="18" y2="18" />
    </svg>
  ),
  Camera: () => (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z" />
      <circle cx="12" cy="13" r="4" />
    </svg>
  ),
  Edit: () => (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
      <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
    </svg>
  ),
  Share: () => (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <circle cx="18" cy="5" r="3" />
      <circle cx="6" cy="12" r="3" />
      <circle cx="18" cy="19" r="3" />
      <line x1="8.59" y1="13.51" x2="15.42" y2="17.49" />
      <line x1="15.41" y1="6.51" x2="8.59" y2="10.49" />
    </svg>
  ),
  Heart: () => (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z" />
    </svg>
  ),
  Bookmark: () => (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" />
    </svg>
  ),
  Send: () => (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <line x1="22" y1="2" x2="11" y2="13" />
      <polygon points="22 2 15 22 11 13 2 9 22 2" />
    </svg>
  ),
};

function RadialActionMenu({
  items = [],
  position = 'bottom-right',
  accentColor = '#F43F5E',
  radius = 100,
  startAngle = 180,
  endAngle = 270,
}) {
  const [isOpen, setIsOpen] = useState(false);
  const [hoveredIndex, setHoveredIndex] = useState(null);

  const toggleMenu = useCallback(() => {
    setIsOpen(prev => !prev);
    setHoveredIndex(null);
  }, []);

  const handleItemClick = useCallback((item, index) => {
    item.onClick?.();
    setIsOpen(false);
  }, []);

  // Calculate position for each item in the arc
  const getItemPosition = (index, total) => {
    const angleRange = endAngle - startAngle;
    const angleStep = total > 1 ? angleRange / (total - 1) : 0;
    const angle = startAngle + (angleStep * index);
    const radian = (angle * Math.PI) / 180;
    
    return {
      x: Math.cos(radian) * radius,
      y: Math.sin(radian) * radius,
      angle: angle,
    };
  };

  // Position classes based on corner
  const positionClasses = {
    'bottom-right': 'bottom-8 right-8',
    'bottom-left': 'bottom-8 left-8',
    'top-right': 'top-8 right-8',
    'top-left': 'top-8 left-8',
  };

  // Adjust angles based on position
  const getAdjustedAngles = () => {
    switch (position) {
      case 'bottom-right': return { start: 180, end: 270 };
      case 'bottom-left': return { start: 270, end: 360 };
      case 'top-right': return { start: 90, end: 180 };
      case 'top-left': return { start: 0, end: 90 };
      default: return { start: startAngle, end: endAngle };
    }
  };

  const angles = getAdjustedAngles();

  // Determine if tooltip should appear on the right side
  const isLeftSide = position === 'bottom-left' || position === 'top-left';

  return (
    <div className={`fixed ${positionClasses[position]} z-50`}>
      {/* Backdrop blur when open */}
      <div 
        className={`fixed inset-0 transition-all duration-300 pointer-events-none ${
          isOpen ? 'bg-slate-900/20 backdrop-blur-sm' : 'bg-transparent'
        }`}
        style={{ zIndex: -1 }}
      />

      {/* Menu Items */}
      <div className="relative">
        {items.map((item, index) => {
          const pos = getItemPosition(index, items.length);
          const isHovered = hoveredIndex === index;
          
          return (
            <div
              key={index}
              className="absolute"
              style={{
                transform: isOpen 
                  ? `translate(${pos.x}px, ${pos.y}px) scale(1) rotate(0deg)`
                  : 'translate(0, 0) scale(0) rotate(-180deg)',
                transition: `all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1)`,
                transitionDelay: isOpen ? `${index * 50}ms` : `${(items.length - index) * 30}ms`,
                left: '50%',
                top: '50%',
                marginLeft: '-24px',
                marginTop: '-24px',
              }}
            >
              {/* Tooltip Label */}
              <div
                className={`absolute whitespace-nowrap px-3 py-1.5 rounded-lg text-sm font-medium transition-all duration-200 pointer-events-none ${
                  isHovered && isOpen ? 'opacity-100' : 'opacity-0'
                }`}
                style={{
                  ...(isLeftSide 
                    ? { left: 'calc(100% + 12px)' }
                    : { right: 'calc(100% + 12px)' }
                  ),
                  top: '50%',
                  transform: `translateY(-50%) translateX(${
                    isHovered && isOpen 
                      ? '0' 
                      : (isLeftSide ? '-8px' : '8px')
                  })`,
                  backgroundColor: 'rgba(15, 23, 42, 0.9)',
                  color: 'white',
                  backdropFilter: 'blur(8px)',
                }}
              >
                {item.label}
                {/* Tooltip arrow */}
                <div 
                  className="absolute w-2 h-2 rotate-45"
                  style={{
                    ...(isLeftSide
                      ? { left: '-4px' }
                      : { right: '-4px' }
                    ),
                    top: '50%',
                    marginTop: '-4px',
                    backgroundColor: 'rgba(15, 23, 42, 0.9)',
                  }}
                />
              </div>

              {/* Action Button */}
              <button
                onClick={() => handleItemClick(item, index)}
                onMouseEnter={() => setHoveredIndex(index)}
                onMouseLeave={() => setHoveredIndex(null)}
                className="w-12 h-12 rounded-full flex items-center justify-center transition-all duration-200 shadow-lg hover:shadow-xl"
                style={{
                  backgroundColor: isHovered ? (item.color || accentColor) : 'white',
                  color: isHovered ? 'white' : (item.color || accentColor),
                  transform: isHovered ? 'scale(1.15)' : 'scale(1)',
                  boxShadow: isHovered 
                    ? `0 8px 30px -5px ${item.color || accentColor}50`
                    : '0 4px 15px -3px rgba(0,0,0,0.1)',
                }}
              >
                {item.icon}
              </button>
            </div>
          );
        })}

        {/* Main FAB Button */}
        <button
          onClick={toggleMenu}
          className="relative w-14 h-14 rounded-full flex items-center justify-center shadow-xl transition-all duration-300 hover:shadow-2xl"
          style={{
            backgroundColor: accentColor,
            color: 'white',
            transform: isOpen ? 'rotate(135deg) scale(0.9)' : 'rotate(0deg) scale(1)',
            boxShadow: `0 10px 40px -10px ${accentColor}80`,
          }}
        >
          {/* Ripple ring effect */}
          <div 
            className="absolute inset-0 rounded-full transition-all duration-500"
            style={{
              border: `2px solid ${accentColor}`,
              transform: isOpen ? 'scale(1.8)' : 'scale(1)',
              opacity: isOpen ? 0 : 0.3,
            }}
          />
          <Icons.Plus />
        </button>
      </div>
    </div>
  );
}

// ============================================
// DEMO SHOWCASE
// ============================================

export default function RadialMenuDemo() {
  const [lastAction, setLastAction] = useState(null);
  const [showNotification, setShowNotification] = useState(false);

  const showActionFeedback = (action) => {
    setLastAction(action);
    setShowNotification(true);
    setTimeout(() => setShowNotification(false), 2000);
  };

  // Example menu configurations
  const primaryMenuItems = [
    { icon: <Icons.Camera />, label: 'Take Photo', onClick: () => showActionFeedback('Camera opened'), color: '#8B5CF6' },
    { icon: <Icons.Edit />, label: 'New Note', onClick: () => showActionFeedback('Creating note...'), color: '#3B82F6' },
    { icon: <Icons.Share />, label: 'Share', onClick: () => showActionFeedback('Share dialog opened'), color: '#10B981' },
    { icon: <Icons.Heart />, label: 'Favorite', onClick: () => showActionFeedback('Added to favorites'), color: '#F43F5E' },
    { icon: <Icons.Bookmark />, label: 'Save', onClick: () => showActionFeedback('Saved!'), color: '#F59E0B' },
  ];

  const secondaryMenuItems = [
    { icon: <Icons.Send />, label: 'Send Message', onClick: () => showActionFeedback('Message sent!') },
    { icon: <Icons.Edit />, label: 'Edit', onClick: () => showActionFeedback('Edit mode') },
    { icon: <Icons.Share />, label: 'Share', onClick: () => showActionFeedback('Sharing...') },
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-rose-50 to-amber-50 overflow-hidden">
      {/* Decorative Background Elements */}
      <div className="fixed inset-0 pointer-events-none overflow-hidden">
        <div className="absolute top-20 left-20 w-72 h-72 bg-rose-200/30 rounded-full blur-3xl" />
        <div className="absolute bottom-40 right-40 w-96 h-96 bg-amber-200/30 rounded-full blur-3xl" />
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] bg-violet-200/20 rounded-full blur-3xl" />
        
        {/* Grid pattern */}
        <div 
          className="absolute inset-0 opacity-[0.03]"
          style={{
            backgroundImage: `
              linear-gradient(rgba(0,0,0,0.1) 1px, transparent 1px),
              linear-gradient(90deg, rgba(0,0,0,0.1) 1px, transparent 1px)
            `,
            backgroundSize: '60px 60px',
          }}
        />
      </div>

      {/* Content */}
      <div className="relative z-10 max-w-4xl mx-auto px-8 py-16">
        {/* Header */}
        <header className="text-center mb-16">
          <div className="inline-flex items-center gap-2 px-4 py-2 bg-white/60 backdrop-blur-sm rounded-full border border-rose-200/50 mb-6">
            <span className="w-2 h-2 bg-rose-500 rounded-full animate-pulse" />
            <span className="text-sm text-slate-600 tracking-wide" style={{ fontFamily: "'JetBrains Mono', monospace" }}>
              Reusable Component
            </span>
          </div>
          
          <h1 
            className="text-5xl md:text-6xl font-bold text-slate-800 mb-4 tracking-tight"
            style={{ fontFamily: "'Outfit', sans-serif" }}
          >
            Radial Action
            <span className="block text-transparent bg-clip-text bg-gradient-to-r from-rose-500 to-amber-500">
              Menu
            </span>
          </h1>
          
          <p className="text-lg text-slate-500 max-w-xl mx-auto leading-relaxed">
            A floating action button that blooms into a radial menu with spring-physics animations. 
            Fully customizable position, colors, and actions.
          </p>
        </header>

        {/* Props Documentation Card */}
        <div className="bg-white/70 backdrop-blur-xl rounded-3xl border border-white/50 shadow-xl shadow-slate-200/50 p-8 mb-12">
          <h2 
            className="text-2xl font-semibold text-slate-800 mb-6"
            style={{ fontFamily: "'Outfit', sans-serif" }}
          >
            Component API
          </h2>
          
          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead>
                <tr className="border-b border-slate-200">
                  <th className="py-3 px-4 text-sm font-semibold text-slate-600">Prop</th>
                  <th className="py-3 px-4 text-sm font-semibold text-slate-600">Type</th>
                  <th className="py-3 px-4 text-sm font-semibold text-slate-600">Default</th>
                  <th className="py-3 px-4 text-sm font-semibold text-slate-600">Description</th>
                </tr>
              </thead>
              <tbody className="text-sm" style={{ fontFamily: "'JetBrains Mono', monospace" }}>
                {[
                  { prop: 'items', type: 'Array', default: '[]', desc: 'Menu items with icon, label, onClick, color' },
                  { prop: 'position', type: 'string', default: "'bottom-right'", desc: 'Corner placement of the FAB' },
                  { prop: 'accentColor', type: 'string', default: "'#F43F5E'", desc: 'Primary color for the main button' },
                  { prop: 'radius', type: 'number', default: '100', desc: 'Distance of items from center (px)' },
                ].map((row, i) => (
                  <tr key={i} className="border-b border-slate-100 hover:bg-slate-50/50 transition-colors">
                    <td className="py-3 px-4 text-rose-600">{row.prop}</td>
                    <td className="py-3 px-4 text-slate-500">{row.type}</td>
                    <td className="py-3 px-4 text-amber-600">{row.default}</td>
                    <td className="py-3 px-4 text-slate-600 font-sans">{row.desc}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {/* Usage Example */}
        <div className="bg-slate-900 rounded-3xl p-6 shadow-2xl">
          <div className="flex items-center gap-2 mb-4">
            <div className="w-3 h-3 rounded-full bg-rose-500" />
            <div className="w-3 h-3 rounded-full bg-amber-500" />
            <div className="w-3 h-3 rounded-full bg-emerald-500" />
            <span className="ml-4 text-slate-500 text-sm" style={{ fontFamily: "'JetBrains Mono', monospace" }}>
              usage.jsx
            </span>
          </div>
          
          <pre className="text-sm overflow-x-auto" style={{ fontFamily: "'JetBrains Mono', monospace" }}>
            <code>
              <span className="text-slate-500">{'// Import the component\n'}</span>
              <span className="text-violet-400">import</span>
              <span className="text-slate-300">{' RadialActionMenu '}</span>
              <span className="text-violet-400">from</span>
              <span className="text-amber-300">{" './RadialActionMenu'"}</span>
              <span className="text-slate-300">;</span>
              {'\n\n'}
              <span className="text-slate-500">{'// Define your menu items\n'}</span>
              <span className="text-violet-400">const</span>
              <span className="text-slate-300">{' menuItems = [\n'}</span>
              <span className="text-slate-300">{'  { '}</span>
              <span className="text-rose-400">icon</span>
              <span className="text-slate-300">{': <CameraIcon />, '}</span>
              <span className="text-rose-400">label</span>
              <span className="text-slate-300">{': '}</span>
              <span className="text-amber-300">"Photo"</span>
              <span className="text-slate-300">{' },\n'}</span>
              <span className="text-slate-300">{'  { '}</span>
              <span className="text-rose-400">icon</span>
              <span className="text-slate-300">{': <EditIcon />, '}</span>
              <span className="text-rose-400">label</span>
              <span className="text-slate-300">{': '}</span>
              <span className="text-amber-300">"Edit"</span>
              <span className="text-slate-300">{' },\n'}</span>
              <span className="text-slate-300">{'];\n\n'}</span>
              <span className="text-slate-500">{'// Use in your app\n'}</span>
              <span className="text-slate-300">{'<'}</span>
              <span className="text-emerald-400">RadialActionMenu</span>
              {'\n'}
              <span className="text-slate-300">{'  '}</span>
              <span className="text-rose-400">items</span>
              <span className="text-slate-300">{'={'}</span>
              <span className="text-slate-100">{'menuItems'}</span>
              <span className="text-slate-300">{'}'}</span>
              {'\n'}
              <span className="text-slate-300">{'  '}</span>
              <span className="text-rose-400">position</span>
              <span className="text-slate-300">{'='}</span>
              <span className="text-amber-300">"bottom-right"</span>
              {'\n'}
              <span className="text-slate-300">{'  '}</span>
              <span className="text-rose-400">accentColor</span>
              <span className="text-slate-300">{'='}</span>
              <span className="text-amber-300">"#F43F5E"</span>
              {'\n'}
              <span className="text-slate-300">{'/>'}</span>
            </code>
          </pre>
        </div>

        {/* Interactive hint */}
        <div className="text-center mt-12">
          <p className="text-slate-400 text-sm">
            ↘ Click the floating button in the corner to try it out
          </p>
        </div>
      </div>

      {/* Action Feedback Notification */}
      <div
        className={`fixed top-8 left-1/2 -translate-x-1/2 z-50 transition-all duration-500 ${
          showNotification ? 'opacity-100 translate-y-0' : 'opacity-0 -translate-y-4 pointer-events-none'
        }`}
      >
        <div className="px-6 py-3 bg-slate-900 text-white rounded-full shadow-xl flex items-center gap-3">
          <span className="w-2 h-2 bg-emerald-400 rounded-full animate-pulse" />
          <span style={{ fontFamily: "'Outfit', sans-serif" }}>{lastAction}</span>
        </div>
      </div>

      {/* The Radial Menu Component - Primary Demo */}
      <RadialActionMenu
        items={primaryMenuItems}
        position="bottom-right"
        accentColor="#F43F5E"
        radius={100}
      />

      {/* Secondary Demo - Different position and color */}
      <RadialActionMenu
        items={secondaryMenuItems}
        position="bottom-left"
        accentColor="#8B5CF6"
        radius={90}
      />

      {/* Custom Fonts */}
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Outfit:wght@400;500;600;700&family=JetBrains+Mono:wght@400;500&display=swap');
      `}</style>
    </div>
  );
}
