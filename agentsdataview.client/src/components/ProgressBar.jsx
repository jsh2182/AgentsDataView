import { useEffect, useState } from "react";

const ProgressBar = ({ duration = 8000, trigger = 0, color = "#0d6efd", height = 4 }) => {
  const [key, setKey] = useState(0);

  // وقتی trigger تغییر کند، انیمیشن reset می‌شود
  useEffect(() => {
    setKey(prev => prev + 1);
  }, [trigger]);

  return (
    <div className="progress-bar-container" style={{ height }}>
      <div
        key={key}
        className="progress-bar-fill"
        
        style={{
          animationDuration: `${duration}ms`,
          backgroundColor: color,
        }}
      />
    </div>
  );
};

export default ProgressBar;
